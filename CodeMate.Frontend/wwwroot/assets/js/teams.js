(() => {
    const STATUS = {
        1: { label: "در انتظار", cls: "status-pending" },
        2: { label: "قبول شد", cls: "status-active" },
        3: { label: "رد شد", cls: "status-off" },
    };
    const EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

    const content = CodeMateShell.mount({
        active: "teams",
        eyebrow: "تیم‌ها",
        title: "درخواست‌های عضویت",
    });

    content.innerHTML = `
    <div class="panel">
      <h3>درخواست‌هایی که فرستادم</h3>
      <p class="panel-desc">برای عضو شدن توی یه پروژه، از «پروژه‌ها» روی «تیم‌ها»ی اون پروژه بزن و درخواست عضویت بده. پروژه باید «فعال» باشه.</p>
      <div id="banner" class="banner"></div>
      <div id="holder"></div>
    </div>
  `;

    const banner = document.getElementById("banner");
    const holder = document.getElementById("holder");

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

    function rowHtml(r) {
        const st = STATUS[r.status] || { label: String(r.status), cls: "status-draft" };
        const hasProject = r.projectId && r.projectId !== EMPTY_GUID;
        const title = r.projectTitle || (hasProject ? shortId(r.projectId) : "پروژه");
        const team = r.teamName || shortId(r.teamId);
        return `<li>
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(title)}</span>
      <span style="color:var(--muted);font-size:0.8rem;">تیم: ${escapeHtml(team)}</span>
      ${r.message ? `<span style="color:var(--muted);font-size:0.8rem;max-width:30%;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;" title="${escapeHtml(r.message)}">${escapeHtml(r.message)}</span>` : ""}
      <span class="status-badge ${st.cls}">${st.label}</span>
      ${hasProject ? `<a class="btn btn-line btn-sm-line" href="project-teams.html?id=${encodeURIComponent(r.projectId)}">تیم‌های پروژه</a>` : ""}
    </li>`;
    }

    async function load() {
        CodeMateForm.hideBanner(banner);
        holder.innerHTML = Array.from({ length: 3 })
            .map(() => `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div>`)
            .join("");
        try {
            const data = await CodeMateApi.get("/join-requests/mine", { auth: true });
            const list = (Array.isArray(data) ? data : []).sort((a, b) => {
                const pa = a.status === 1 ? 0 : 1;
                const pb = b.status === 1 ? 0 : 1;
                return pa - pb;
            });
            if (!list.length) {
                holder.innerHTML = `<div class="empty-state">هنوز درخواستی نفرستادی.</div>`;
                return;
            }
            holder.innerHTML = `<ul class="project-list is-rich">${list.map(rowHtml).join("")}</ul>`;
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(banner, errText(err));
        }
    }

    load();
})();
