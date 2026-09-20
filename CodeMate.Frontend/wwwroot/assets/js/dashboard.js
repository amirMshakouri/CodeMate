        (() => {
        const session = CodeMateApi.getSession();
        const content = CodeMateShell.mount({
            active: "dashboard",
            eyebrow: "نمای کلی",
            title: `خوش برگشتی، ${session ? session.userName : ""}`,
        });

        content.innerHTML = `
    <div class="stat-row" id="stat-row">
      ${Array.from({ length: 4 })
                .map(() => `<div class="stat-card"><div class="skeleton" style="height:14px;width:60%;margin-bottom:10px;"></div><div class="skeleton" style="height:28px;width:40%;"></div></div>`)
                .join("")}
    </div>
    <div class="panel">
      <h3>پروژه‌های تو</h3>
      <p class="panel-desc">جزئیات کامل پروژه‌ها با ماژول Projects اضافه می‌شه — فعلاً این‌هایی هستن که توشون هستی.</p>
      <div id="projects-holder"><div class="skeleton" style="height:60px;"></div></div>
    </div>
  `;

        async function load() {
            try {
                const data = await CodeMateApi.get("/dashboard/me", { auth: true });
                renderStats(data);
                renderProjects(data.myProjects || []);
            } catch (err) {
                document.getElementById("stat-row").innerHTML = `
        <div class="empty-state" style="grid-column: 1 / -1;">
          داشبورد لود نشد: ${escapeHtml(err.message || "خطای ناشناخته")}
        </div>`;
            }
        }

        function renderStats(data) {
            const total = data.myTasksTotal ?? 0;
            const completed = data.myTasksCompleted ?? 0;
            const overdue = data.myTasksOverdue ?? 0;
            const projectCount = (data.myProjects || []).length;

            document.getElementById("stat-row").innerHTML = `
      <div class="stat-card">
        <div class="stat-label">تسک‌های محول‌شده</div>
        <div class="stat-value">${total}</div>
      </div>
      <div class="stat-card is-accent">
        <div class="stat-label">تکمیل‌شده</div>
        <div class="stat-value">${completed}</div>
      </div>
      <div class="stat-card ${overdue > 0 ? "is-warn" : ""}">
        <div class="stat-label">عقب‌افتاده</div>
        <div class="stat-value">${overdue}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">پروژه‌های فعال</div>
        <div class="stat-value">${projectCount}</div>
      </div>
    `;
        }

        async function renderProjects(projectIds) {
            const holder = document.getElementById("projects-holder");
            if (!projectIds.length) {
                holder.innerHTML = `<div class="empty-state">هنوز توی هیچ پروژه‌ای نیستی.</div>`;
                return;
            }
            const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();
            const results = await Promise.allSettled(
                projectIds.map((id) => CodeMateApi.get(`/projects/${id}`, { auth: true }))
            );
            holder.innerHTML = `<ul class="project-list is-rich">
      ${projectIds
          .map((id, i) => {
              const r = results[i];
              const project = r.status === "fulfilled" ? r.value : null;
              const title = project ? project.title : id;
              const isOwner = project && String(project.ownerId).toLowerCase() === myId;
              return `<li>
          <span class="project-dot"></span>
          <span class="project-title">${escapeHtml(title)}</span>
          ${isOwner ? `<a class="btn btn-line btn-sm-line" href="project-dashboard.html?id=${encodeURIComponent(id)}">داشبورد پروژه</a>` : ""}
        </li>`;
          })
          .join("")}
    </ul>`;
        }

        function escapeHtml(str) {
            const div = document.createElement("div");
            div.textContent = String(str);
            return div.innerHTML;
        }

        load();
    })();
