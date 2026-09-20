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

    const content = CodeMateShell.mount({
        active: "tasks",
        eyebrow: "تسک‌ها",
        title: "تسک‌های من",
    });

    content.innerHTML = `
    <div class="panel">
      <h3>تسک‌هایی که به من Assign شده</h3>
      <p class="panel-desc">وضعیت تسک فقط به‌ترتیب جلو می‌ره: انجام‌نشده ← در حال انجام ← انجام‌شده.</p>
      <div id="filters" class="d-flex gap-2 flex-wrap"></div>
      <div id="banner" class="banner" style="margin-top:1rem;"></div>
      <div id="holder" style="margin-top:1rem;"></div>
    </div>
  `;

    const banner = document.getElementById("banner");
    const holder = document.getElementById("holder");
    const filtersEl = document.getElementById("filters");

    let tasks = [];
    let projectTitles = {};
    let filter = "all";

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
        if (!value) return "";
        const d = new Date(value);
        return isNaN(d) || d.getFullYear() < 1990 ? "" : d.toLocaleDateString("fa-IR");
    }

    function isOverdue(t) {
        return !!t.dueDate && new Date(t.dueDate) < new Date() && t.status !== 2;
    }

    async function load() {
        CodeMateForm.hideBanner(banner);
        holder.innerHTML = Array.from({ length: 3 })
            .map(() => `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div>`)
            .join("");
        try {
            const data = await CodeMateApi.get("/tasks/mine", { auth: true });
            tasks = Array.isArray(data) ? data : [];

            // اسم پروژه‌ها (هر پروژه یک‌بار)
            const ids = [...new Set(tasks.map((t) => t.projectId))];
            const results = await Promise.allSettled(ids.map((id) => CodeMateApi.get(`/projects/${id}`, { auth: true })));
            projectTitles = {};
            ids.forEach((id, i) => {
                projectTitles[id] = results[i].status === "fulfilled" ? results[i].value.title : "پروژه‌ی حذف‌شده";
            });

            render();
        } catch (err) {
            holder.innerHTML = "";
            filtersEl.innerHTML = "";
            CodeMateForm.showBanner(banner, errText(err));
        }
    }

    function renderFilters() {
        const count = (s) => tasks.filter((t) => String(t.status) === s).length;
        const items = [
            ["all", `همه (${tasks.length})`],
            ["0", `انجام‌نشده (${count("0")})`],
            ["1", `در حال انجام (${count("1")})`],
            ["2", `انجام‌شده (${count("2")})`],
        ];
        filtersEl.innerHTML = items
            .map(([key, label]) => `<button type="button" class="btn ${filter === key ? "btn-accent" : "btn-line"}" data-filter="${key}">${label}</button>`)
            .join("");
    }

    function sorted(list) {
        return [...list].sort((a, b) => {
            const doneA = a.status === 2 ? 1 : 0;
            const doneB = b.status === 2 ? 1 : 0;
            if (doneA !== doneB) return doneA - doneB;
            const da = a.dueDate ? new Date(a.dueDate).getTime() : Infinity;
            const db = b.dueDate ? new Date(b.dueDate).getTime() : Infinity;
            return da - db;
        });
    }

    function rowHtml(t) {
        const st = TASK_STATUS[t.status] || { label: String(t.status), cls: "status-draft" };
        const pr = PRIORITY[t.priority] || { label: String(t.priority), cls: "status-draft" };
        const overdue = isOverdue(t);
        const due = fmtDate(t.dueDate);
        const action =
            t.status === 0
                ? `<button type="button" class="btn btn-accent btn-sm-line" data-act="next" data-next="1">شروع</button>`
                : t.status === 1
                ? `<button type="button" class="btn btn-accent btn-sm-line" data-act="next" data-next="2">انجام شد</button>`
                : "";
        return `<li data-id="${escapeHtml(t.id)}" data-title="${escapeHtml(t.title)}">
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(t.title)}</span>
      <span style="color:var(--muted);font-size:0.8rem;">${escapeHtml(projectTitles[t.projectId] || "")}</span>
      <span class="status-badge ${pr.cls}">اولویت ${pr.label}</span>
      ${due ? `<span class="status-badge ${overdue ? "status-off" : "status-draft"}">${overdue ? "عقب‌افتاده — " : "سررسید: "}${due}</span>` : ""}
      <span class="status-badge ${st.cls}">${st.label}</span>
      <button type="button" class="btn btn-line btn-sm-line" data-act="details">جزئیات</button>
      ${action}
    </li>`;
    }

    function render() {
        renderFilters();
        const list = sorted(filter === "all" ? tasks : tasks.filter((t) => String(t.status) === filter));
        if (!list.length) {
            holder.innerHTML = `<div class="empty-state">${
                tasks.length ? "تسکی با این وضعیت نداری." : "هنوز تسکی به تو Assign نشده."
            }</div>`;
            return;
        }
        holder.innerHTML = `<ul class="project-list is-rich">${list.map(rowHtml).join("")}</ul>`;
    }

    filtersEl.addEventListener("click", (e) => {
        const btn = e.target.closest("button[data-filter]");
        if (!btn) return;
        filter = btn.dataset.filter;
        render();
    });


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

    holder.addEventListener("click", async (e) => {
        const detailsBtn = e.target.closest("button[data-act='details']");
        if (detailsBtn) {
            await toggleDetails(detailsBtn, detailsBtn.closest("li"), banner);
            return;
        }
        const btn = e.target.closest("button[data-act='next']");
        if (!btn) return;
        const li = btn.closest("li");
        const { id, title } = li.dataset;
        const next = Number(btn.dataset.next);

        if (next === 2 && !window.confirm(`«${title}» انجام‌شده حساب بشه؟ بعدش نمی‌تونی وضعیتش رو برگردونی.`)) return;

        CodeMateForm.hideBanner(banner);
        btn.disabled = true;
        try {
            await CodeMateApi.patch(`/tasks/${id}/status`, { status: next }, { auth: true });
            await load();
            CodeMateForm.showBanner(banner, `«${title}» شد «${TASK_STATUS[next].label}».`, "ok");
        } catch (err) {
            btn.disabled = false;
            CodeMateForm.showBanner(banner, errText(err));
        }
    });

    load();
})();
