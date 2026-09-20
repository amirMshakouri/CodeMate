(() => {
    const STATUS = {
        1: { label: "پیش‌نویس", cls: "status-draft" },
        2: { label: "در انتظار", cls: "status-pending" },
        3: { label: "فعال", cls: "status-active" },
        4: { label: "تکمیل‌شده", cls: "status-done" },
    };
    const PAGE_SIZE = 8;

    const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();
    const isAdmin = CodeMateApi.isAdmin();

    const content = CodeMateShell.mount({
        active: "projects",
        eyebrow: "پروژه‌ها",
        title: "همه‌ی پروژه‌ها",
    });

    content.innerHTML = `
    <div class="panel">
      <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
        <div>
          <h3>پروژه‌ها</h3>
          <p class="panel-desc" style="margin-bottom:0;">پروژه‌ها رو ببین، یا یه پروژه‌ی جدید بساز و مدیریتش کن.</p>
        </div>
        <button type="button" class="btn btn-accent" id="toggle-create-btn">پروژه‌ی جدید</button>
      </div>

      <form id="create-form" style="display:none; margin-top:1.2rem;" novalidate>
        <div id="create-banner" class="banner"></div>
        <div class="row">
          <div class="col-md-5 mb-3">
            <label class="form-label" for="new-title">عنوان</label>
            <input type="text" class="form-control" id="new-title" maxlength="150">
            <div class="field-error" data-for="title"></div>
          </div>
          <div class="col-md-7 mb-3">
            <label class="form-label" for="new-description">توضیحات (اختیاری)</label>
            <input type="text" class="form-control" id="new-description" maxlength="1000">
            <div class="field-error" data-for="description"></div>
          </div>
        </div>
        <button type="submit" class="btn btn-accent" id="create-btn">ساخت پروژه</button>
      </form>
    </div>

    <div class="panel">
      <form id="filter-form" class="row g-2 align-items-end" novalidate>
        <div class="col-md-6">
          <label class="form-label" for="filter-title">جستجو در عنوان</label>
          <input type="text" class="form-control" id="filter-title" placeholder="مثلاً Smoke">
        </div>
        <div class="col-md-3">
          <label class="form-label" for="filter-status">وضعیت</label>
          <select class="form-control" id="filter-status">
            <option value="">همه</option>
            ${Object.entries(STATUS)
                .map(([value, s]) => `<option value="${value}">${s.label}</option>`)
                .join("")}
          </select>
        </div>
        <div class="col-md-3 d-flex gap-2">
          <button type="submit" class="btn btn-line flex-fill">جستجو</button>
          <button type="button" class="btn btn-line" id="reset-btn">پاک‌کردن</button>
        </div>
      </form>

      <p class="panel-desc" style="margin:1rem 0 0;font-size:0.82rem;">
        <strong>فعال:</strong> پروژه درخواست عضویت می‌پذیره. &nbsp;
        <strong>تکمیل‌شده:</strong> تسک‌های پروژه قفل می‌شن. &nbsp;
        پیش‌نویس و در انتظار: هنوز عضو نمی‌گیره.
      </p>

      <div id="list-banner" class="banner" style="margin-top:1rem;"></div>
      <div id="list-holder" style="margin-top:1rem;"></div>
      <div id="pager" class="pager" style="display:none;"></div>
    </div>
  `;

    const listBanner = document.getElementById("list-banner");
    const holder = document.getElementById("list-holder");
    const pager = document.getElementById("pager");
    const createForm = document.getElementById("create-form");
    const createBanner = document.getElementById("create-banner");
    const filterForm = document.getElementById("filter-form");

    const state = { page: 1, title: "", status: "" };

    function escapeHtml(str) {
        const div = document.createElement("div");
        div.textContent = String(str ?? "");
        return div.innerHTML;
    }

    function errText(err) {
        if (err && err.errors) {
            const first = Object.values(err.errors).flat()[0];
            if (first) return first;
        }
        return (err && err.message) || "خطای ناشناخته";
    }

    function skeleton() {
        return Array.from({ length: 3 })
            .map(() => `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div>`)
            .join("");
    }

    async function load() {
        CodeMateForm.hideBanner(listBanner);
        holder.innerHTML = skeleton();
        pager.style.display = "none";

        const params = new URLSearchParams({
            PageNumber: state.page,
            PageSize: PAGE_SIZE,
        });
        if (state.title) params.set("Title", state.title);
        if (state.status) params.set("Status", state.status);

        try {
            const data = await CodeMateApi.get(`/projects?${params.toString()}`, { auth: true });
            let items = data.items || [];

            // اگه کارت پروژه ownerId نداشت، از جزئیات هر پروژه می‌گیریم تا بدونیم مال کیه
            if (items.some((p) => p.ownerId === undefined)) {
                const details = await Promise.allSettled(
                    items.map((p) => CodeMateApi.get(`/projects/${p.id}`, { auth: true }))
                );
                items = items.map((p, i) => ({
                    ...p,
                    ownerId: details[i].status === "fulfilled" ? details[i].value.ownerId : p.ownerId,
                }));
            }

            renderList(items);
            renderPager(data);
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(listBanner, errText(err) || "لیست پروژه‌ها لود نشد.");
        }
    }

    function statusOptions(current) {
        return Object.entries(STATUS)
            .map(([v, s]) => `<option value="${v}" ${String(current) === v ? "selected" : ""}>${s.label}</option>`)
            .join("");
    }

    function rowHtml(p) {
        const s = STATUS[p.status] || { label: String(p.status), cls: "status-draft" };
        const mine = !!p.ownerId && String(p.ownerId).toLowerCase() === myId;
        const canDashboard = mine || isAdmin;

        const statusControl = mine
            ? `<select class="form-control" style="padding:0.2rem 0.5rem;font-size:0.82rem;width:auto;" data-act="status" data-prev="${escapeHtml(p.status)}">${statusOptions(p.status)}</select>`
            : `<span class="status-badge ${s.cls}">${s.label}</span>`;

        return `<li data-id="${escapeHtml(p.id)}" data-title="${escapeHtml(p.title)}">
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(p.title)}</span>
      ${mine ? '<span class="status-badge status-done">پروژه‌ی من</span>' : ""}
      ${statusControl}
      <a class="btn btn-line btn-sm-line" href="project-teams.html?id=${encodeURIComponent(p.id)}">تیم‌ها</a>
      <a class="btn btn-line btn-sm-line" href="project-tasks.html?id=${encodeURIComponent(p.id)}">تسک‌ها</a>
      ${canDashboard ? `<a class="btn btn-line btn-sm-line" href="project-dashboard.html?id=${encodeURIComponent(p.id)}">داشبورد</a>` : ""}
      ${
          mine
              ? `<button type="button" class="btn btn-line btn-sm-line" data-act="edit">ویرایش</button>
             <button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="delete">حذف</button>`
              : ""
      }
    </li>`;
    }

    function renderList(items) {
        if (!items.length) {
            holder.innerHTML = `<div class="empty-state">${
                state.title || state.status ? "پروژه‌ای با این فیلترها پیدا نشد." : "هنوز پروژه‌ای ثبت نشده."
            }</div>`;
            return;
        }
        holder.innerHTML = `<ul class="project-list is-rich">${items.map(rowHtml).join("")}</ul>`;
    }

    function renderPager(data) {
        const totalPages = data.totalPages || 1;
        if (totalPages <= 1) {
            pager.style.display = "none";
            return;
        }
        pager.style.display = "flex";
        pager.innerHTML = `
      <button type="button" class="btn btn-line" id="next-btn" ${state.page >= totalPages ? "disabled" : ""}>بعدی</button>
      <span>صفحه ${state.page} از ${totalPages} — ${data.totalCount} پروژه</span>
      <button type="button" class="btn btn-line" id="prev-btn" ${state.page <= 1 ? "disabled" : ""}>قبلی</button>
    `;
        document.getElementById("prev-btn").addEventListener("click", () => {
            state.page -= 1;
            load();
        });
        document.getElementById("next-btn").addEventListener("click", () => {
            state.page += 1;
            load();
        });
    }

    // ---- تغییر وضعیت (فقط صاحب پروژه؛ بک‌اند هم چک می‌کنه)
    holder.addEventListener("change", async (e) => {
        const sel = e.target.closest("select[data-act='status']");
        if (!sel) return;
        const li = sel.closest("li");
        const { id, title } = li.dataset;
        const next = sel.value;
        const prev = sel.dataset.prev;
        if (next === prev) return;

        const label = STATUS[next].label;
        const message =
            next === "4"
                ? `«${title}» تکمیل‌شده بشه؟ بعدش تسک‌های پروژه دیگه قابل ساخت و تغییر نیست.`
                : next === "3"
                ? `«${title}» فعال بشه؟ از این به بعد درخواست عضویت می‌پذیره.`
                : `وضعیت «${title}» به «${label}» تغییر کنه؟ تا فعال نشه، درخواست عضویت جدید نمی‌پذیره.`;
        if (!window.confirm(message)) {
            sel.value = prev;
            return;
        }

        CodeMateForm.hideBanner(listBanner);
        sel.disabled = true;
        try {
            await CodeMateApi.patch(`/projects/${id}/status?status=${encodeURIComponent(next)}`, undefined, { auth: true });
            await load();
            CodeMateForm.showBanner(listBanner, `وضعیت «${title}» شد «${label}».`, "ok");
        } catch (err) {
            sel.disabled = false;
            sel.value = prev;
            CodeMateForm.showBanner(listBanner, errText(err));
        }
    });

    // ---- ویرایش / حذف
    holder.addEventListener("click", async (e) => {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const li = btn.closest("li");
        const { id, title } = li.dataset;
        const act = btn.dataset.act;

        if (act === "edit") {
            const existing = li.querySelector(".project-edit");
            if (existing) {
                existing.remove();
                return;
            }
            btn.disabled = true;
            try {
                const d = await CodeMateApi.get(`/projects/${id}`, { auth: true });
                const box = document.createElement("div");
                box.className = "project-edit";
                box.innerHTML = `
          <div class="row g-2 align-items-end">
            <div class="col-md-4">
              <label class="form-label">عنوان</label>
              <input type="text" class="form-control" maxlength="150" data-role="title" value="${escapeHtml(d.title)}">
            </div>
            <div class="col-md-5">
              <label class="form-label">توضیحات</label>
              <input type="text" class="form-control" maxlength="1000" data-role="desc" value="${escapeHtml(d.description || "")}">
            </div>
            <div class="col-md-3 d-flex gap-2">
              <button type="button" class="btn btn-accent flex-fill" data-act="save">ذخیره</button>
              <button type="button" class="btn btn-line" data-act="cancel">انصراف</button>
            </div>
          </div>`;
                li.appendChild(box);
            } catch (err) {
                CodeMateForm.showBanner(listBanner, errText(err));
            } finally {
                btn.disabled = false;
            }
            return;
        }

        if (act === "cancel") {
            li.querySelector(".project-edit")?.remove();
            return;
        }

        if (act === "save") {
            const box = li.querySelector(".project-edit");
            const newTitle = box.querySelector("[data-role='title']").value.trim();
            const description = box.querySelector("[data-role='desc']").value.trim();
            CodeMateForm.hideBanner(listBanner);
            if (newTitle.length < 3 || newTitle.length > 150) {
                CodeMateForm.showBanner(listBanner, "عنوان باید بین ۳ تا ۱۵۰ کاراکتر باشه.");
                return;
            }
            btn.disabled = true;
            try {
                await CodeMateApi.put(`/projects/${id}`, { title: newTitle, description: description || null }, { auth: true });
                await load();
                CodeMateForm.showBanner(listBanner, "پروژه ویرایش شد.", "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(listBanner, errText(err));
            }
            return;
        }

        if (act === "delete") {
            if (!window.confirm(`پروژه‌ی «${title}» حذف بشه؟`)) return;
            CodeMateForm.hideBanner(listBanner);
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/projects/${id}`, { auth: true });
                await load();
                CodeMateForm.showBanner(listBanner, `«${title}» حذف شد.`, "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(listBanner, errText(err));
            }
        }
    });

    filterForm.addEventListener("submit", (e) => {
        e.preventDefault();
        state.title = document.getElementById("filter-title").value.trim();
        state.status = document.getElementById("filter-status").value;
        state.page = 1;
        load();
    });

    document.getElementById("reset-btn").addEventListener("click", () => {
        document.getElementById("filter-title").value = "";
        document.getElementById("filter-status").value = "";
        state.title = "";
        state.status = "";
        state.page = 1;
        load();
    });

    document.getElementById("toggle-create-btn").addEventListener("click", () => {
        const hidden = createForm.style.display === "none";
        createForm.style.display = hidden ? "block" : "none";
        if (hidden) document.getElementById("new-title").focus();
    });

    createForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        CodeMateForm.hideBanner(createBanner);
        CodeMateForm.clearFieldErrors(createForm);

        const title = document.getElementById("new-title").value.trim();
        const description = document.getElementById("new-description").value.trim();

        if (title.length < 3) {
            const target = createForm.querySelector('.field-error[data-for="title"]');
            target.textContent = "عنوان باید حداقل ۳ کاراکتر باشه.";
            target.style.display = "block";
            return;
        }

        const btn = document.getElementById("create-btn");
        CodeMateForm.setLoading(btn, true, "در حال ساخت…");
        try {
            await CodeMateApi.post("/projects", { title, description: description || null }, { auth: true });
            createForm.reset();
            createForm.style.display = "none";
            state.page = 1;
            await load();
            CodeMateForm.showBanner(
                listBanner,
                "پروژه به‌صورت پیش‌نویس ساخته شد. برای اینکه بقیه بتونن درخواست عضویت بدن، وضعیتش رو «فعال» کن.",
                "ok"
            );
        } catch (err) {
            if (!CodeMateForm.applyFieldErrors(createForm, err.errors)) {
                CodeMateForm.showBanner(createBanner, errText(err) || "ساخت پروژه ناموفق بود.");
            }
        } finally {
            CodeMateForm.setLoading(btn, false);
        }
    });

    load();
})();
