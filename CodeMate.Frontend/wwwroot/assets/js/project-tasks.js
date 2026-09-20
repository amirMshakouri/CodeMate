(() => {
    const TASK_STATUS = {
        0: { label: "انجام‌نشده", cls: "status-draft" },
        1: { label: "در حال انجام", cls: "status-pending" },
        2: { label: "انجام‌شده", cls: "status-active" },
    };
    const PRIORITY = {
        1: { label: "کم", cls: "status-draft" },
        2: { label: "متوسط", cls: "status-pending" },
        3: { label: "زیاد", cls: "status-off" },
    };
    const PAGE_SIZE = 10;

    const projectId = new URLSearchParams(window.location.search).get("id");
    const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();
    const isAdmin = CodeMateApi.isAdmin();

    const content = CodeMateShell.mount({
        active: "projects",
        eyebrow: "تسک‌های پروژه",
        title: "…",
    });

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

    function shortId(id) {
        return String(id).slice(0, 8) + "…";
    }

    function fmtDate(value) {
        if (!value) return "";
        const d = new Date(value);
        return isNaN(d) || d.getFullYear() < 1990 ? "" : d.toLocaleDateString("fa-IR");
    }

    function dateInputValue(value) {
        if (!value) return "";
        const d = new Date(value);
        return isNaN(d) || d.getFullYear() < 1990 ? "" : d.toISOString().slice(0, 10);
    }

    // تاریخ انتخاب‌شده رو ظهر UTC می‌گیریم تا با قانون «سررسید باید توی آینده باشه» راحت‌تر کنار بیاد
    function toDueIso(dateStr) {
        return new Date(dateStr + "T12:00:00Z").toISOString();
    }

    function tomorrowStr() {
        return new Date(Date.now() + 86400e3).toISOString().slice(0, 10);
    }

    function message(text) {
        content.innerHTML = `
      <div class="panel">
        <div class="empty-state">${escapeHtml(text)}</div>
        <div style="text-align:center;margin-top:1rem;">
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>`;
    }

    const state = { project: null, members: [], names: {}, isOwner: false, locked: false, page: 1, status: "" };

    // ---------- لود اولیه ----------
    async function init() {
        if (!projectId) {
            message("پروژه‌ای انتخاب نشده.");
            return;
        }
        content.innerHTML = `<div class="skeleton" style="height:100px;margin-bottom:12px;"></div><div class="skeleton" style="height:200px;"></div>`;

        try {
            state.project = await CodeMateApi.get(`/projects/${encodeURIComponent(projectId)}`, { auth: true });
        } catch (err) {
            message(err.status === 404 ? "این پروژه پیدا نشد." : errText(err));
            return;
        }

        const project = state.project;
        const h1 = document.querySelector(".topbar h1");
        if (h1) h1.textContent = project.title;
        state.isOwner = String(project.ownerId).toLowerCase() === myId;
        state.locked = project.status === 4;

        await loadMembers();
        buildLayout();
        await loadTasks();
    }

    async function loadMembers() {
        state.members = [];
        state.names = {};
        try {
            const teams = await CodeMateApi.get(`/projects/${encodeURIComponent(projectId)}/teams`, { auth: true });
            const lists = await Promise.allSettled(
                (teams || []).map((t) => CodeMateApi.get(`/teams/${t.id}/members`, { auth: true }))
            );
            const seen = new Set();
            lists.forEach((r) => {
                if (r.status !== "fulfilled") return;
                (r.value || []).forEach((m) => {
                    const key = String(m.userId).toLowerCase();
                    if (seen.has(key)) return;
                    seen.add(key);
                    const name = m.userName || shortId(m.userId);
                    state.members.push({ userId: m.userId, name });
                    state.names[key] = name;
                });
            });
        } catch {
            /* اسم اعضا اختیاریه؛ اگه نیومد با شناسه‌ی کوتاه نمایش می‌دیم */
        }
    }

    // ---------- چیدمان ----------
    function buildLayout() {
        const project = state.project;
        const canCreate = state.isOwner && !state.locked;
        const canDashboard = state.isOwner || isAdmin;

        content.innerHTML = `
      <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
        <div class="d-flex align-items-center gap-2 flex-wrap">
          ${state.isOwner ? '<span class="status-badge status-done">پروژه‌ی من</span>' : ""}
          ${state.locked ? '<span class="status-badge status-off">تکمیل‌شده — تسک‌ها قفل هستن</span>' : ""}
          <span style="color:var(--ink-soft);">${escapeHtml(project.description || "بدون توضیحات")}</span>
        </div>
        <div class="d-flex gap-2">
          <a href="project-teams.html?id=${encodeURIComponent(project.id)}" class="btn btn-line">تیم‌ها</a>
          ${canDashboard ? `<a href="project-dashboard.html?id=${encodeURIComponent(project.id)}" class="btn btn-line">داشبورد</a>` : ""}
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>

      ${
          canCreate
              ? `<div class="panel">
        <div class="d-flex justify-content-between align-items-center">
          <h3 style="margin:0;">تسک جدید</h3>
          <button type="button" class="btn btn-accent" id="toggle-create-btn">افزودن تسک</button>
        </div>
        <form id="create-form" style="display:none;margin-top:1rem;" novalidate>
          <div id="create-banner" class="banner"></div>
          <div class="row g-2 align-items-end">
            <div class="col-md-4">
              <label class="form-label" for="c-title">عنوان</label>
              <input type="text" class="form-control" id="c-title" maxlength="150">
            </div>
            <div class="col-md-3">
              <label class="form-label" for="c-desc">توضیحات</label>
              <input type="text" class="form-control" id="c-desc" maxlength="1000">
            </div>
            <div class="col-md-2">
              <label class="form-label" for="c-priority">اولویت</label>
              <select class="form-control" id="c-priority">
                <option value="1">کم</option><option value="2" selected>متوسط</option><option value="3">زیاد</option>
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label" for="c-due">سررسید</label>
              <input type="date" class="form-control" id="c-due" min="${tomorrowStr()}">
            </div>
            <div class="col-md-1">
              <button type="submit" class="btn btn-accent w-100" id="create-btn">ثبت</button>
            </div>
          </div>
        </form>
      </div>`
              : ""
      }

      <div class="panel">
        <div class="row g-2 align-items-end">
          <div class="col-md-4">
            <label class="form-label" for="f-status">وضعیت</label>
            <select class="form-control" id="f-status">
              <option value="">همه</option>
              ${Object.entries(TASK_STATUS).map(([v, s]) => `<option value="${v}">${s.label}</option>`).join("")}
            </select>
          </div>
        </div>
        <div id="banner" class="banner" style="margin-top:1rem;"></div>
        <div id="holder" style="margin-top:1rem;"></div>
        <div id="pager" class="pager" style="display:none;"></div>
      </div>`;

        document.getElementById("f-status").addEventListener("change", (e) => {
            state.status = e.target.value;
            state.page = 1;
            loadTasks();
        });

        const createForm = document.getElementById("create-form");
        if (createForm) {
            document.getElementById("toggle-create-btn").addEventListener("click", () => {
                const hidden = createForm.style.display === "none";
                createForm.style.display = hidden ? "block" : "none";
                if (hidden) document.getElementById("c-title").focus();
            });
            createForm.addEventListener("submit", onCreate);
        }

        document.getElementById("holder").addEventListener("click", onHolderClick);
        document.getElementById("holder").addEventListener("change", onAssignChange);
    }

    // ---------- لیست تسک‌ها ----------
    async function loadTasks() {
        const banner = document.getElementById("banner");
        const holder = document.getElementById("holder");
        const pager = document.getElementById("pager");
        CodeMateForm.hideBanner(banner);
        holder.innerHTML = Array.from({ length: 3 })
            .map(() => `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div>`)
            .join("");
        pager.style.display = "none";

        const params = new URLSearchParams({ ProjectId: projectId, PageNumber: state.page, PageSize: PAGE_SIZE });
        if (state.status !== "") params.set("Status", state.status);

        try {
            const data = await CodeMateApi.get(`/tasks?${params.toString()}`, { auth: true });
            renderTasks(data.items || []);
            renderPager(data);
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(banner, errText(err));
        }
    }

    function assigneeName(userId) {
        if (!userId) return null;
        return state.names[String(userId).toLowerCase()] || shortId(userId);
    }

    function assignControl(t) {
        if (!(state.isOwner && !state.locked)) {
            const name = assigneeName(t.assignedUserId);
            return `<span style="color:var(--muted);font-size:0.8rem;">${name ? "مسئول: " + escapeHtml(name) : "بدون مسئول"}</span>`;
        }
        if (!state.members.length) {
            return `<span style="color:var(--muted);font-size:0.8rem;">عضوی برای Assign نیست</span>`;
        }
        const current = t.assignedUserId ? String(t.assignedUserId).toLowerCase() : "";
        const known = state.members.some((m) => String(m.userId).toLowerCase() === current);
        const options = [
            `<option value="" disabled ${current ? "" : "selected"}>Assign به…</option>`,
            ...(current && !known ? [`<option value="${escapeHtml(t.assignedUserId)}" selected>${escapeHtml(shortId(t.assignedUserId))}</option>`] : []),
            ...state.members.map(
                (m) =>
                    `<option value="${escapeHtml(m.userId)}" ${String(m.userId).toLowerCase() === current ? "selected" : ""}>${escapeHtml(m.name)}</option>`
            ),
        ].join("");
        return `<select class="form-control" style="padding:0.2rem 0.5rem;font-size:0.82rem;width:auto;" data-act="assign" data-prev="${escapeHtml(t.assignedUserId || "")}">${options}</select>`;
    }

    function rowHtml(t) {
        const st = TASK_STATUS[t.status] || { label: String(t.status), cls: "status-draft" };
        const pr = PRIORITY[t.priority] || { label: String(t.priority), cls: "status-draft" };
        const due = fmtDate(t.dueDate);
        const overdue = !!t.dueDate && new Date(t.dueDate) < new Date() && t.status !== 2 && !!due;
        const isAssignee = !!t.assignedUserId && String(t.assignedUserId).toLowerCase() === myId;
        const canAdvance = !state.locked && (state.isOwner || isAssignee) && t.status < 2;
        const advance = canAdvance
            ? `<button type="button" class="btn btn-accent btn-sm-line" data-act="next" data-next="${t.status + 1}">${t.status === 0 ? "شروع" : "انجام شد"}</button>`
            : "";
        const ownerButtons =
            state.isOwner && !state.locked
                ? `<button type="button" class="btn btn-line btn-sm-line" data-act="edit">ویرایش</button>
           <button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="delete">حذف</button>`
                : "";
        return `<li data-id="${escapeHtml(t.id)}" data-title="${escapeHtml(t.title)}">
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(t.title)}</span>
      <span class="status-badge ${pr.cls}">اولویت ${pr.label}</span>
      ${due ? `<span class="status-badge ${overdue ? "status-off" : "status-draft"}">${overdue ? "عقب‌افتاده — " : "سررسید: "}${due}</span>` : ""}
      <span class="status-badge ${st.cls}">${st.label}</span>
      ${assignControl(t)}
      <button type="button" class="btn btn-line btn-sm-line" data-act="details">جزئیات</button>
      ${advance}
      ${ownerButtons}
    </li>`;
    }

    function renderTasks(items) {
        const holder = document.getElementById("holder");
        if (!items.length) {
            holder.innerHTML = `<div class="empty-state">${
                state.status !== "" ? "تسکی با این وضعیت نیست." : "این پروژه هنوز تسکی نداره."
            }</div>`;
            return;
        }
        holder.innerHTML = `<ul class="project-list is-rich">${items.map(rowHtml).join("")}</ul>`;
    }

    function renderPager(data) {
        const pager = document.getElementById("pager");
        const totalPages = data.totalPages || 1;
        if (totalPages <= 1) {
            pager.style.display = "none";
            return;
        }
        pager.style.display = "flex";
        pager.innerHTML = `
      <button type="button" class="btn btn-line" id="next-btn" ${state.page >= totalPages ? "disabled" : ""}>بعدی</button>
      <span>صفحه ${state.page} از ${totalPages} — ${data.totalCount} تسک</span>
      <button type="button" class="btn btn-line" id="prev-btn" ${state.page <= 1 ? "disabled" : ""}>قبلی</button>`;
        document.getElementById("prev-btn").addEventListener("click", () => { state.page -= 1; loadTasks(); });
        document.getElementById("next-btn").addEventListener("click", () => { state.page += 1; loadTasks(); });
    }

    // ---------- ساخت تسک ----------
    async function onCreate(e) {
        e.preventDefault();
        const banner = document.getElementById("create-banner");
        const listBanner = document.getElementById("banner");
        CodeMateForm.hideBanner(banner);

        const title = document.getElementById("c-title").value.trim();
        const description = document.getElementById("c-desc").value.trim();
        const priority = Number(document.getElementById("c-priority").value);
        const due = document.getElementById("c-due").value;

        if (!title || title.length > 150) {
            CodeMateForm.showBanner(banner, "عنوان اجباریه و نباید بیشتر از ۱۵۰ کاراکتر باشه.");
            return;
        }
        if (due && new Date(toDueIso(due)) <= new Date()) {
            CodeMateForm.showBanner(banner, "سررسید باید توی آینده باشه.");
            return;
        }

        const btn = document.getElementById("create-btn");
        CodeMateForm.setLoading(btn, true, "…");
        try {
            await CodeMateApi.post(
                "/tasks",
                { projectId, title, description: description || null, priority, dueDate: due ? toDueIso(due) : null },
                { auth: true }
            );
            document.getElementById("create-form").reset();
            document.getElementById("create-form").style.display = "none";
            state.page = 1;
            await loadTasks();
            CodeMateForm.showBanner(listBanner, `تسک «${title}» ساخته شد. با منوی کنار تسک می‌تونی به یکی از اعضا Assign کنی.`, "ok");
        } catch (err) {
            CodeMateForm.showBanner(banner, errText(err));
        } finally {
            CodeMateForm.setLoading(btn, false);
        }
    }

    // ---------- Assign ----------
    async function onAssignChange(e) {
        const sel = e.target.closest("select[data-act='assign']");
        if (!sel) return;
        const banner = document.getElementById("banner");
        const li = sel.closest("li");
        const { id, title } = li.dataset;
        const prev = sel.dataset.prev;
        const next = sel.value;
        if (!next || next === prev) return;

        CodeMateForm.hideBanner(banner);
        sel.disabled = true;
        try {
            await CodeMateApi.patch(`/tasks/${id}/assign`, { assignedUserId: next }, { auth: true });
            sel.dataset.prev = next;
            sel.disabled = false;
            CodeMateForm.showBanner(banner, `«${title}» به ${assigneeName(next)} Assign شد.`, "ok");
        } catch (err) {
            sel.disabled = false;
            sel.value = prev;
            CodeMateForm.showBanner(banner, errText(err));
        }
    }


    // ---- جزئیات تسک (توضیحات فقط توی GET /tasks/{id} هست، نه توی لیست)
    async function toggleDetails(btn, li, banner) {
        const existing = li.querySelector(".task-details");
        if (existing) {
            existing.remove();
            return;
        }
        btn.disabled = true;
        try {
            const d = await CodeMateApi.get(`/tasks/${li.dataset.id}`, { auth: true });
            const box = document.createElement("div");
            box.className = "project-edit task-details";
            const text = d.description && d.description.trim() ? escapeHtml(d.description) : "بدون توضیحات";
            box.innerHTML = `<div style="background:var(--paper);border:1px solid var(--line);border-radius:8px;padding:0.7rem 0.9rem;font-size:0.88rem;color:var(--ink-soft);">
        <strong style="color:var(--ink);">توضیحات:</strong>
        <div style="white-space:pre-wrap;margin-top:0.3rem;">${text}</div>
      </div>`;
            li.appendChild(box);
        } catch (err) {
            CodeMateForm.showBanner(banner, errText(err));
        } finally {
            btn.disabled = false;
        }
    }

    // ---------- شروع / انجام / ویرایش / حذف ----------
    async function onHolderClick(e) {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const banner = document.getElementById("banner");
        const li = btn.closest("li");
        const { id, title } = li.dataset;
        const act = btn.dataset.act;

        if (act === "details") {
            await toggleDetails(btn, li, banner);
            return;
        }

        if (act === "next") {
            const next = Number(btn.dataset.next);
            if (next === 2 && !window.confirm(`«${title}» انجام‌شده حساب بشه؟ بعدش نمی‌تونی وضعیتش رو برگردونی.`)) return;
            CodeMateForm.hideBanner(banner);
            btn.disabled = true;
            try {
                await CodeMateApi.patch(`/tasks/${id}/status`, { status: next }, { auth: true });
                await loadTasks();
                CodeMateForm.showBanner(banner, `«${title}» شد «${TASK_STATUS[next].label}».`, "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }

        if (act === "edit") {
            const existing = li.querySelector(".project-edit");
            if (existing) {
                existing.remove();
                return;
            }
            btn.disabled = true;
            try {
                const d = await CodeMateApi.get(`/tasks/${id}`, { auth: true });
                const box = document.createElement("div");
                box.className = "project-edit";
                box.dataset.originalDue = dateInputValue(d.dueDate);
                box.innerHTML = `
          <div class="row g-2 align-items-end">
            <div class="col-md-3">
              <label class="form-label">عنوان</label>
              <input type="text" class="form-control" maxlength="150" data-role="title" value="${escapeHtml(d.title)}">
            </div>
            <div class="col-md-3">
              <label class="form-label">توضیحات</label>
              <input type="text" class="form-control" maxlength="1000" data-role="desc" value="${escapeHtml(d.description || "")}">
            </div>
            <div class="col-md-2">
              <label class="form-label">اولویت</label>
              <select class="form-control" data-role="priority">
                ${[1, 2, 3].map((v) => `<option value="${v}" ${d.priority === v ? "selected" : ""}>${PRIORITY[v].label}</option>`).join("")}
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label">سررسید</label>
              <input type="date" class="form-control" data-role="due" value="${dateInputValue(d.dueDate)}">
            </div>
            <div class="col-md-2 d-flex gap-2">
              <button type="button" class="btn btn-accent flex-fill" data-act="save">ذخیره</button>
              <button type="button" class="btn btn-line" data-act="cancel">انصراف</button>
            </div>
          </div>`;
                li.appendChild(box);
            } catch (err) {
                CodeMateForm.showBanner(banner, errText(err));
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
            const priority = Number(box.querySelector("[data-role='priority']").value);
            const due = box.querySelector("[data-role='due']").value;
            CodeMateForm.hideBanner(banner);

            if (!newTitle || newTitle.length > 150) {
                CodeMateForm.showBanner(banner, "عنوان اجباریه و نباید بیشتر از ۱۵۰ کاراکتر باشه.");
                return;
            }
            // سررسید فقط وقتی فرستاده می‌شه که عوض شده باشه (بک‌اند تاریخ گذشته رو رد می‌کنه و null رو نادیده می‌گیره)
            const dueChanged = due && due !== box.dataset.originalDue;
            if (dueChanged && new Date(toDueIso(due)) <= new Date()) {
                CodeMateForm.showBanner(banner, "سررسید باید توی آینده باشه.");
                return;
            }

            btn.disabled = true;
            try {
                await CodeMateApi.put(
                    `/tasks/${id}`,
                    { title: newTitle, description, priority, dueDate: dueChanged ? toDueIso(due) : null },
                    { auth: true }
                );
                await loadTasks();
                CodeMateForm.showBanner(banner, "تسک ویرایش شد.", "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }

        if (act === "delete") {
            if (!window.confirm(`تسک «${title}» حذف بشه؟`)) return;
            CodeMateForm.hideBanner(banner);
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/tasks/${id}`, { auth: true });
                await loadTasks();
                CodeMateForm.showBanner(banner, `«${title}» حذف شد.`, "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
        }
    }

    init();
})();
