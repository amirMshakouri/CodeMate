(() => {
    const STATUS = {
        1: { label: "پیش‌نویس", cls: "status-draft" },
        2: { label: "در انتظار", cls: "status-pending" },
        3: { label: "فعال", cls: "status-active" },
        4: { label: "تکمیل‌شده", cls: "status-done" },
    };

    const projectId = new URLSearchParams(window.location.search).get("id");
    const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();

    const content = CodeMateShell.mount({
        active: "projects",
        eyebrow: "داشبورد پروژه",
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

    function message(text) {
        content.innerHTML = `
      <div class="panel">
        <div class="empty-state">${escapeHtml(text)}</div>
        <div style="text-align:center;margin-top:1rem;">
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>`;
    }

    function shortId(id) {
        return String(id).slice(0, 8) + "…";
    }

    function statCard(label, value, extraClass) {
        return `<div class="stat-card ${extraClass || ""}">
      <div class="stat-label">${label}</div>
      <div class="stat-value">${value}</div>
    </div>`;
    }

    function render(project, stats) {
        const s = STATUS[project.status] || { label: String(project.status), cls: "status-draft" };
        const total = stats.totalTasks ?? 0;
        const done = stats.doneCount ?? 0;
        const pct = Math.max(0, Math.min(100, Number(stats.progressPercentage) || 0));
        const members = stats.memberPerformance || [];
        const asAdmin = CodeMateApi.isAdmin() && String(project.ownerId).toLowerCase() !== myId;

        content.innerHTML = `
      <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
        <div class="d-flex align-items-center gap-2 flex-wrap">
          <span class="status-badge ${s.cls}">${s.label}</span>
          ${asAdmin ? '<span class="status-badge status-done">نمای ادمین</span>' : ""}
          <span style="color:var(--ink-soft);">${escapeHtml(project.description || "بدون توضیحات")}</span>
        </div>
        <div class="d-flex gap-2">
          <a href="project-tasks.html?id=${encodeURIComponent(project.id)}" class="btn btn-line">تسک‌های پروژه</a>
          <a href="project-teams.html?id=${encodeURIComponent(project.id)}" class="btn btn-line">تیم‌ها</a>
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>

      <div class="stat-row stat-row-5">
        ${statCard("کل تسک‌ها", total)}
        ${statCard("انجام‌نشده", stats.todoCount ?? 0)}
        ${statCard("در حال انجام", stats.doingCount ?? 0)}
        ${statCard("انجام‌شده", done, "is-accent")}
        ${statCard("عقب‌افتاده", stats.overdueCount ?? 0, (stats.overdueCount ?? 0) > 0 ? "is-warn" : "")}
      </div>

      <div class="panel">
        <h3>پیشرفت پروژه</h3>
        <div class="progress-track"><div class="progress-fill" style="width:${pct}%"></div></div>
        <p class="panel-desc" style="margin:0.6rem 0 0;">
          ${pct.toLocaleString("fa-IR")}٪ — ${done.toLocaleString("fa-IR")} از ${total.toLocaleString("fa-IR")} تسک انجام شده
        </p>
      </div>

      <div class="panel">
        <h3>عملکرد اعضا</h3>
        <p class="panel-desc">تعداد تسک‌های انجام‌شده‌ی هر عضو.</p>
        ${
            members.length
                ? `<ul class="project-list is-rich">
          ${members
              .map((m) => {
                  const isMe = String(m.userId).toLowerCase() === myId;
                  const name = m.userName ? m.userName : shortId(m.userId);
                  return `<li>
              <span class="project-dot"></span>
              <span class="project-title" title="${escapeHtml(m.userId)}">${escapeHtml(name)}${isMe ? " (تو)" : ""}</span>
              <span class="status-badge status-active">${Number(m.completedTasksCount ?? 0).toLocaleString("fa-IR")} تسک</span>
            </li>`;
              })
              .join("")}
        </ul>`
                : `<div class="empty-state">هنوز تسکی انجام نشده.</div>`
        }
      </div>`;
    }

    async function load() {
        if (!projectId) {
            message("پروژه‌ای انتخاب نشده.");
            return;
        }
        content.innerHTML = `<div class="skeleton" style="height:120px;margin-bottom:12px;"></div><div class="skeleton" style="height:200px;"></div>`;

        let project;
        try {
            project = await CodeMateApi.get(`/projects/${encodeURIComponent(projectId)}`, { auth: true });
        } catch (err) {
            message(err.status === 404 ? "این پروژه پیدا نشد." : errText(err));
            return;
        }

        const h1 = document.querySelector(".topbar h1");
        if (h1) h1.textContent = project.title;
        document.title = `${project.title} — CodeMate`;

        // داشبورد فقط برای Owner پروژه‌ست؛ قبل از صدا زدن API پیام روشن نشون می‌دیم
        if (!CodeMateApi.isAdmin() && String(project.ownerId).toLowerCase() !== myId) {
            message("داشبورد پروژه فقط برای صاحب پروژه در دسترسه.");
            return;
        }

        try {
            const stats = await CodeMateApi.get(`/dashboard/projects/${encodeURIComponent(projectId)}`, { auth: true });
            render(project, stats);
        } catch (err) {
            message(err.status === 403 ? "داشبورد پروژه فقط برای صاحب پروژه در دسترسه." : errText(err));
        }
    }

    load();
})();
