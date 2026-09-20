        const CodeMateShell = (() => {
            const NAV_ITEMS = [
                { key: "dashboard", label: "داشبورد", href: "dashboard.html", enabled: true },
                { key: "profile", label: "پروفایل و مهارت‌ها", href: "profile.html", enabled: true },
                { key: "projects", label: "پروژه‌ها", href: "projects.html", enabled: true },
                { key: "teams", label: "درخواست‌های عضویت", href: "teams.html", enabled: true },
                { key: "tasks", label: "تسک‌های من", href: "tasks.html", enabled: true },
            ];

            const ADMIN_ITEMS = [
                { key: "admin-users", label: "مدیریت کاربران", href: "admin-users.html", enabled: true },
                { key: "admin-projects", label: "نظارت بر پروژه‌ها", href: "admin-projects.html", enabled: true },
                { key: "admin-skills", label: "کاتالوگ مهارت‌ها", href: "admin-skills.html", enabled: true },
            ];

            function initials(name) {
                if (!name) return "?";
                return name.trim().slice(0, 2).toUpperCase();
            }

            function mount({ active, eyebrow, title }) {
                const session = CodeMateApi.getSession();
                const username = session ? session.userName : "";

                const items = CodeMateApi.isAdmin()
                    ? [...NAV_ITEMS, { divider: true, label: "مدیریت" }, ...ADMIN_ITEMS]
                    : NAV_ITEMS;

                const navHtml = items.map((item) => {
                    if (item.divider) {
                        return `<li class="nav-item" style="padding:1rem 0.9rem 0.3rem;font-size:0.72rem;color:#5c6067;">${item.label}</li>`;
                    }
                    if (!item.enabled) {
                        return `<li class="nav-item">
          <a href="${item.href}" class="disabled" onclick="return false;">
            <span>${item.label}</span>
            <span class="soon-badge">به‌زودی</span>
          </a>
        </li>`;
                    }
                    const activeClass = item.key === active ? "active" : "";
                    return `<li class="nav-item">
        <a href="${item.href}" class="${activeClass}">${item.label}</a>
      </li>`;
                }).join("");

                const shell = document.createElement("div");
                shell.className = "app-shell";
                shell.innerHTML = `
      <aside class="app-sidebar">
        <div class="wordmark">Code<span>Mate</span></div>
        <ul class="nav-group">${navHtml}</ul>
        <div class="sidebar-foot">
          <div class="mono" style="color:#5c6067;">API: connected</div>
        </div>
      </aside>
      <main class="app-main">
        <div class="topbar">
          <div>
            <div class="eyebrow">${eyebrow || ""}</div>
            <h1>${title || ""}</h1>
          </div>
          <div class="user-chip">
            <div class="avatar">${initials(username)}</div>
            <div>
              <div style="font-weight:600;">${username || ""}</div>
              <a href="#" id="cm-logout" style="font-size:0.82rem;">خروج</a>
            </div>
          </div>
        </div>
        <div id="cm-main-content"></div>
      </main>
    `;

                document.body.prepend(shell);

                document.getElementById("cm-logout").addEventListener("click", async (e) => {
                    e.preventDefault();
                    try {
                        await CodeMateApi.post("/auth/logout", undefined, { auth: true });
                    } catch {
                        // توکن ممکنه از قبل منقضی شده باشه، مهم نیست
                    }
                    CodeMateApi.clearSession();
                    window.location.href = "login.html";
                });

                return document.getElementById("cm-main-content");
            }

            return { mount };
        })();
