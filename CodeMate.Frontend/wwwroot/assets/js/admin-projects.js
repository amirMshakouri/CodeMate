(() => {
    // فقط برای ادمین. کنترل اصلی دسترسی سمت بک‌اند انجام می‌شه؛ این برای راحتی UIه.
    if (!CodeMateApi.isAdmin()) {
        window.location.href = "dashboard.html";
        return;
    }

    const PAGE_SIZE = 8;
    const STATUS = {
        1: { label: "پیش‌نویس", cls: "status-draft" },
        2: { label: "در انتظار", cls: "status-pending" },
        3: { label: "فعال", cls: "status-active" },
        4: { label: "تکمیل‌شده", cls: "status-done" },
    };

    const content = CodeMateShell.mount({
        active: "admin-projects",
        eyebrow: "مدیریت",
        title: "نظارت بر پروژه‌ها",
    });

    content.innerHTML = `
    <div class="panel">
      <h3>همه‌ی پروژه‌ها</h3>
      <p class="panel-desc">ادمین می‌تونه وضعیت هر پروژه رو تغییر بده، ویرایشش کنه، حذفش کنه یا داشبوردش رو ببینه.</p>

      <form id="filter-form" class="row g-2 align-items-end" novalidate>
        <div class="col-md-6">
          <label class="form-label" for="f-title">جستجو در عنوان</label>
          <input type="text" class="form-control" id="f-title">
        </div>
        <div class="col-md-3">
          <label class="form-label" for="f-status">وضعیت</label>
          <select class="form-control" id="f-status">
            <option value="">همه</option>
            ${Object.entries(STATUS).map(([v, s]) => `<option value="${v}">${s.label}</option>`).join("")}
          </select>
        </div>
        <div class="col-md-3 d-flex gap-2">
          <button type="submit" class="btn btn-line flex-fill">جستجو</button>
          <button type="button" class="btn btn-line" id="reset-btn">پاک‌کردن</button>
        </div>
      </form>

      <div id="banner" class="banner" style="margin-top:1rem;"></div>
      <div id="table-holder" style="margin-top:1rem;overflow-x:auto;"></div>
      <div id="pager" class="pager" style="display:none;"></div>
    </div>
  `;

    const banner = document.getElementById("banner");
    const holder = document.getElementById("table-holder");
    const pager = document.getElementById("pager");
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

    function fmtDate(value) {
        if (!value) return "—";
        const d = new Date(value);
        return isNaN(d) || d.getFullYear() < 1990 ? "—" : d.toLocaleDateString("fa-IR");
    }

    async function load() {
        CodeMateForm.hideBanner(banner);
        holder.innerHTML = Array.from({ length: 4 })
            .map(() => `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div>`)
            .join("");
        pager.style.display = "none";

        const params = new URLSearchParams({ PageNumber: state.page, PageSize: PAGE_SIZE });
        if (state.title) params.set("Title", state.title);
        if (state.status) params.set("Status", state.status);

        try {
            const data = await CodeMateApi.get(`/projects?${params.toString()}`, { auth: true });
            const items = data.items || [];

            // کارت پروژه صاحب و توضیحات نداره؛ برای هر ردیف جزئیات رو می‌گیریم
            const details = await Promise.allSettled(
                items.map((p) => CodeMateApi.get(`/projects/${p.id}`, { auth: true }))
            );
            const rows = items.map((p, i) => ({
                ...p,
                ...(details[i].status === "fulfilled" ? details[i].value : {}),
            }));

            // اسم صاحب‌ها از endpoint ادمین
            const ownerIds = [...new Set(rows.map((r) => r.ownerId).filter(Boolean))];
            const owners = await Promise.allSettled(
                ownerIds.map((id) => CodeMateApi.get(`/admin/users/${id}`, { auth: true }))
            );
            const names = {};
            ownerIds.forEach((id, i) => {
                if (owners[i].status === "fulfilled") names[id] = owners[i].value.userName;
            });

            renderTable(rows, names);
            renderPager(data);
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(banner, errText(err));
        }
    }

    function renderTable(rows, names) {
        if (!rows.length) {
            holder.innerHTML = `<div class="empty-state">${
                state.title || state.status ? "پروژه‌ای با این فیلترها پیدا نشد." : "هنوز پروژه‌ای ثبت نشده."
            }</div>`;
            return;
        }
        holder.innerHTML = `
      <table class="data-table">
        <thead>
          <tr><th>پروژه</th><th>صاحب</th><th>وضعیت</th><th>ایجاد</th><th>عملیات</th></tr>
        </thead>
        <tbody>
          ${rows.map((r) => rowHtml(r, names)).join("")}
        </tbody>
      </table>`;
    }

    function rowHtml(r, names) {
        const owner = names[r.ownerId] || (r.ownerId ? String(r.ownerId).slice(0, 8) + "…" : "—");
        const options = Object.entries(STATUS)
            .map(([v, s]) => `<option value="${v}" ${String(r.status) === v ? "selected" : ""}>${s.label}</option>`)
            .join("");
        return `
      <tr data-id="${escapeHtml(r.id)}" data-title="${escapeHtml(r.title)}" data-desc="${escapeHtml(r.description || "")}">
        <td>
          <div style="font-weight:600;">${escapeHtml(r.title)}</div>
          <div style="color:var(--muted);font-size:0.8rem;">${escapeHtml(r.description || "بدون توضیحات")}</div>
        </td>
        <td>${escapeHtml(owner)}</td>
        <td>
          <select class="form-control" style="padding:0.2rem 0.5rem;font-size:0.82rem;width:auto;" data-act="status" data-prev="${escapeHtml(r.status)}">${options}</select>
        </td>
        <td>${fmtDate(r.createdAt)}</td>
        <td class="text-nowrap">
          <a class="btn btn-line btn-sm-line" href="project-dashboard.html?id=${encodeURIComponent(r.id)}">داشبورد</a>
          <button type="button" class="btn btn-line btn-sm-line" data-act="edit">ویرایش</button>
          <button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="delete">حذف</button>
        </td>
      </tr>`;
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
      <button type="button" class="btn btn-line" id="prev-btn" ${state.page <= 1 ? "disabled" : ""}>قبلی</button>`;
        document.getElementById("prev-btn").addEventListener("click", () => { state.page -= 1; load(); });
        document.getElementById("next-btn").addEventListener("click", () => { state.page += 1; load(); });
    }

    // ---- تغییر اجباری وضعیت
    holder.addEventListener("change", async (e) => {
        const sel = e.target.closest("select[data-act='status']");
        if (!sel) return;
        const tr = sel.closest("tr");
        const { id, title } = tr.dataset;
        const next = sel.value;
        const prev = sel.dataset.prev;
        if (next === prev) return;

        if (!window.confirm(`وضعیت «${title}» به «${STATUS[next].label}» تغییر کنه؟`)) {
            sel.value = prev;
            return;
        }
        CodeMateForm.hideBanner(banner);
        sel.disabled = true;
        try {
            await CodeMateApi.patch(`/admin/projects/${id}/force-status?status=${encodeURIComponent(next)}`, undefined, { auth: true });
            await load();
            CodeMateForm.showBanner(banner, `وضعیت «${title}» شد «${STATUS[next].label}».`, "ok");
        } catch (err) {
            sel.disabled = false;
            sel.value = prev;
            CodeMateForm.showBanner(banner, errText(err));
        }
    });

    // ---- ویرایش / حذف
    holder.addEventListener("click", async (e) => {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const tr = btn.closest("tr");
        const act = btn.dataset.act;

        if (act === "edit") {
            const next = tr.nextElementSibling;
            if (next && next.classList.contains("details-row")) {
                next.remove();
                return;
            }
            const row = document.createElement("tr");
            row.className = "details-row";
            row.dataset.forId = tr.dataset.id;
            row.innerHTML = `<td colspan="5">
        <div class="row g-2 align-items-end">
          <div class="col-md-4">
            <label class="form-label">عنوان</label>
            <input type="text" class="form-control" maxlength="150" data-role="title" value="${escapeHtml(tr.dataset.title)}">
          </div>
          <div class="col-md-5">
            <label class="form-label">توضیحات</label>
            <input type="text" class="form-control" maxlength="1000" data-role="desc" value="${escapeHtml(tr.dataset.desc)}">
          </div>
          <div class="col-md-3 d-flex gap-2">
            <button type="button" class="btn btn-accent flex-fill" data-act="save">ذخیره</button>
            <button type="button" class="btn btn-line" data-act="cancel">انصراف</button>
          </div>
        </div>
      </td>`;
            tr.after(row);
            return;
        }

        if (act === "cancel") {
            btn.closest("tr").remove();
            return;
        }

        if (act === "save") {
            const editRow = btn.closest("tr");
            const id = editRow.dataset.forId;
            const title = editRow.querySelector("[data-role='title']").value.trim();
            const description = editRow.querySelector("[data-role='desc']").value.trim();
            CodeMateForm.hideBanner(banner);
            if (title.length < 3 || title.length > 150) {
                CodeMateForm.showBanner(banner, "عنوان باید بین ۳ تا ۱۵۰ کاراکتر باشه.");
                return;
            }
            btn.disabled = true;
            try {
                await CodeMateApi.put(`/projects/${id}`, { title, description: description || null }, { auth: true });
                await load();
                CodeMateForm.showBanner(banner, "پروژه ویرایش شد.", "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }

        if (act === "delete") {
            const { id, title } = tr.dataset;
            if (!window.confirm(`پروژه‌ی «${title}» حذف بشه؟`)) return;
            CodeMateForm.hideBanner(banner);
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/admin/projects/${id}`, { auth: true });
                await load();
                CodeMateForm.showBanner(banner, `«${title}» حذف شد.`, "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
        }
    });

    // ---- فیلتر
    document.getElementById("filter-form").addEventListener("submit", (e) => {
        e.preventDefault();
        state.title = document.getElementById("f-title").value.trim();
        state.status = document.getElementById("f-status").value;
        state.page = 1;
        load();
    });
    document.getElementById("reset-btn").addEventListener("click", () => {
        document.getElementById("f-title").value = "";
        document.getElementById("f-status").value = "";
        state.title = state.status = "";
        state.page = 1;
        load();
    });

    load();
})();
