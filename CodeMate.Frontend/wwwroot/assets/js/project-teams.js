(() => {
    const PROJECT_STATUS = {
        1: { label: "پیش‌نویس", cls: "status-draft" },
        2: { label: "در انتظار", cls: "status-pending" },
        3: { label: "فعال", cls: "status-active" },
        4: { label: "تکمیل‌شده", cls: "status-done" },
    };
    const ROLE = { 1: "عضو", 2: "لیدر" };
    const PENDING = 1;

    const projectId = new URLSearchParams(window.location.search).get("id");
    const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();

    const content = CodeMateShell.mount({
        active: "projects",
        eyebrow: "تیم‌های پروژه",
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

    function message(text) {
        content.innerHTML = `
      <div class="panel">
        <div class="empty-state">${escapeHtml(text)}</div>
        <div style="text-align:center;margin-top:1rem;">
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>`;
    }

    const state = {
        project: null,
        isOwner: false,
        active: false,
        teams: [],
        members: {}, // teamId -> [members]
        pending: {}, // teamId -> [join requests]  (فقط برای صاحب پروژه)
        mine: [], // درخواست‌های خودم
    };

    // ---------- بارگذاری ----------
    async function fetchAll() {
        const teams = await CodeMateApi.get(`/projects/${encodeURIComponent(projectId)}/teams`, { auth: true });
        state.teams = teams || [];

        const memberLists = await Promise.allSettled(
            state.teams.map((t) => CodeMateApi.get(`/teams/${t.id}/members`, { auth: true }))
        );
        state.members = {};
        state.teams.forEach((t, i) => {
            const r = memberLists[i];
            state.members[t.id] = r.status === "fulfilled" ? (r.value || []).filter((m) => m.isActive !== false) : [];
        });

        state.pending = {};
        if (state.isOwner) {
            const pend = await Promise.allSettled(
                state.teams.map((t) => CodeMateApi.get(`/teams/${t.id}/join-requests`, { auth: true }))
            );
            state.teams.forEach((t, i) => {
                state.pending[t.id] = pend[i].status === "fulfilled" ? pend[i].value || [] : [];
            });
        }

        try {
            state.mine = (await CodeMateApi.get("/join-requests/mine", { auth: true })) || [];
        } catch {
            state.mine = [];
        }
    }

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
        const h1 = document.querySelector(".topbar h1");
        if (h1) h1.textContent = state.project.title;
        state.isOwner = String(state.project.ownerId).toLowerCase() === myId;
        state.active = state.project.status === 3;

        try {
            await fetchAll();
        } catch (err) {
            message(errText(err));
            return;
        }
        render();
    }

    async function reload(bannerText) {
        try {
            await fetchAll();
        } catch (err) {
            showBanner(errText(err));
            return;
        }
        render();
        if (bannerText) showBanner(bannerText, "ok");
    }

    function showBanner(text, kind) {
        const b = document.getElementById("banner");
        if (b) CodeMateForm.showBanner(b, text, kind);
    }

    // ---------- رندر ----------
    function isMember(team) {
        return (state.members[team.id] || []).some((m) => String(m.userId).toLowerCase() === myId);
    }

    function hasPending(team) {
        return state.mine.some((r) => r.teamId === team.id && r.status === PENDING);
    }

    function joinControl(team) {
        if (state.isOwner) return "";
        if (isMember(team)) return `<span class="status-badge status-active">عضو این تیم هستی</span>`;
        if (hasPending(team)) return `<span class="status-badge status-pending">درخواست ارسال شده</span>`;
        if (!state.active) return `<span style="color:var(--muted);font-size:0.82rem;">پروژه فعال نیست؛ درخواست عضویت نمی‌پذیره</span>`;
        return `<button type="button" class="btn btn-accent btn-sm-line" data-act="join-open">درخواست عضویت</button>`;
    }

    function ownerTeamButtons() {
        if (!state.isOwner) return "";
        return `<button type="button" class="btn btn-line btn-sm-line" data-act="team-edit">ویرایش تیم</button>
      <button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="team-delete">حذف تیم</button>`;
    }

    function memberRow(team, m) {
        const name = m.userName || shortId(m.userId);
        const isMe = String(m.userId).toLowerCase() === myId;
        return `<li data-user="${escapeHtml(m.userId)}" data-name="${escapeHtml(name)}">
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(name)}${isMe ? " (تو)" : ""}</span>
      <span class="status-badge status-draft">${ROLE[m.role] || ROLE[1]}</span>
      ${state.isOwner ? `<button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="member-remove">حذف از تیم</button>` : ""}
    </li>`;
    }

    function pendingRow(r) {
        const name = r.userName || shortId(r.userId);
        return `<li data-request="${escapeHtml(r.id)}" data-name="${escapeHtml(name)}">
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(name)}</span>
      <span style="color:var(--muted);font-size:0.8rem;max-width:40%;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;" title="${escapeHtml(r.message || "")}">${escapeHtml(r.message || "بدون پیام")}</span>
      <button type="button" class="btn btn-accent btn-sm-line" data-act="req-accept">قبول</button>
      <button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="req-reject">رد</button>
    </li>`;
    }

    function teamPanel(team) {
        const members = state.members[team.id] || [];
        const pending = state.pending[team.id] || [];
        return `<div class="panel" data-team="${escapeHtml(team.id)}" data-name="${escapeHtml(team.name)}" data-desc="${escapeHtml(team.description || "")}">
      <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
        <div>
          <h3 style="margin:0;">${escapeHtml(team.name)}</h3>
          <p class="panel-desc" style="margin:0.2rem 0 0;">${escapeHtml(team.description || "بدون توضیحات")}</p>
        </div>
        <div class="d-flex gap-2 flex-wrap align-items-center">${joinControl(team)}${ownerTeamButtons()}</div>
      </div>
      <div class="team-slot"></div>
      <div style="margin-top:1rem;">
        <strong style="font-size:0.88rem;">اعضا (${members.length.toLocaleString("fa-IR")})</strong>
        ${
            members.length
                ? `<ul class="project-list is-rich">${members.map((m) => memberRow(team, m)).join("")}</ul>`
                : `<div class="empty-state" style="margin-top:0.5rem;">هنوز عضوی نداره.</div>`
        }
      </div>
      ${
          state.isOwner
              ? `<div style="margin-top:1rem;">
        <strong style="font-size:0.88rem;">درخواست‌های در انتظار (${pending.length.toLocaleString("fa-IR")})</strong>
        ${
            pending.length
                ? `<ul class="project-list is-rich">${pending.map(pendingRow).join("")}</ul>`
                : `<div class="empty-state" style="margin-top:0.5rem;">درخواستی در انتظار نیست.</div>`
        }
      </div>`
              : ""
      }
    </div>`;
    }

    function render() {
        const p = state.project;
        const ps = PROJECT_STATUS[p.status] || { label: String(p.status), cls: "status-draft" };
        content.innerHTML = `
      <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
        <div class="d-flex align-items-center gap-2 flex-wrap">
          <span class="status-badge ${ps.cls}">${ps.label}</span>
          ${state.isOwner ? '<span class="status-badge status-done">پروژه‌ی من</span>' : ""}
          <span style="color:var(--ink-soft);">${escapeHtml(p.description || "بدون توضیحات")}</span>
        </div>
        <div class="d-flex gap-2">
          <a href="project-tasks.html?id=${encodeURIComponent(p.id)}" class="btn btn-line">تسک‌ها</a>
          <a href="projects.html" class="btn btn-line">برگشت به پروژه‌ها</a>
        </div>
      </div>

      ${
          !state.active
              ? `<div class="notice-warn" style="background:var(--warn-tint);color:#7a5410;border-radius:8px;padding:0.7rem 1rem;margin-bottom:1rem;font-size:0.88rem;">
        ${state.isOwner ? "این پروژه فعال نیست، پس هنوز درخواست عضویت نمی‌پذیره. از «پروژه‌ها» وضعیتش رو «فعال» کن." : "این پروژه فعال نیست و فعلاً درخواست عضویت نمی‌پذیره."}
      </div>`
              : ""
      }

      ${
          state.isOwner
              ? `<div class="panel">
        <div class="d-flex justify-content-between align-items-center">
          <h3 style="margin:0;">تیم جدید</h3>
          <button type="button" class="btn btn-accent" id="toggle-team-btn">ساخت تیم</button>
        </div>
        <form id="team-form" style="display:none;margin-top:1rem;" novalidate>
          <div id="team-banner" class="banner"></div>
          <div class="row g-2 align-items-end">
            <div class="col-md-4">
              <label class="form-label" for="t-name">نام تیم</label>
              <input type="text" class="form-control" id="t-name" maxlength="100">
            </div>
            <div class="col-md-6">
              <label class="form-label" for="t-desc">توضیحات (اختیاری)</label>
              <input type="text" class="form-control" id="t-desc" maxlength="500">
            </div>
            <div class="col-md-2">
              <button type="submit" class="btn btn-accent w-100" id="team-btn">ثبت</button>
            </div>
          </div>
        </form>
      </div>`
              : ""
      }

      <div id="banner" class="banner" style="margin-bottom:1rem;"></div>
      <div id="teams-holder">
        ${
            state.teams.length
                ? state.teams.map(teamPanel).join("")
                : `<div class="panel"><div class="empty-state">${state.isOwner ? "هنوز تیمی نساختی." : "این پروژه هنوز تیمی نداره."}</div></div>`
        }
      </div>`;

        const form = document.getElementById("team-form");
        if (form) {
            document.getElementById("toggle-team-btn").addEventListener("click", () => {
                const hidden = form.style.display === "none";
                form.style.display = hidden ? "block" : "none";
                if (hidden) document.getElementById("t-name").focus();
            });
            form.addEventListener("submit", onCreateTeam);
        }
        document.getElementById("teams-holder").addEventListener("click", onClick);
    }

    // ---------- ساخت تیم ----------
    async function onCreateTeam(e) {
        e.preventDefault();
        const banner = document.getElementById("team-banner");
        CodeMateForm.hideBanner(banner);
        const name = document.getElementById("t-name").value.trim();
        const description = document.getElementById("t-desc").value.trim();
        if (!name || name.length > 100) {
            CodeMateForm.showBanner(banner, "نام تیم اجباریه و نباید بیشتر از ۱۰۰ کاراکتر باشه.");
            return;
        }
        const btn = document.getElementById("team-btn");
        CodeMateForm.setLoading(btn, true, "…");
        try {
            await CodeMateApi.post("/teams", { projectId, name, description: description || null }, { auth: true });
            await reload(`تیم «${name}» ساخته شد.`);
        } catch (err) {
            CodeMateForm.showBanner(banner, errText(err));
            CodeMateForm.setLoading(btn, false);
        }
    }

    // ---------- کلیک‌ها ----------
    async function onClick(e) {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const panel = btn.closest("[data-team]");
        const teamId = panel.dataset.team;
        const teamName = panel.dataset.name;
        const slot = panel.querySelector(".team-slot");
        const act = btn.dataset.act;
        const banner = document.getElementById("banner");
        CodeMateForm.hideBanner(banner);

        // --- درخواست عضویت
        if (act === "join-open") {
            if (slot.querySelector(".join-box")) {
                slot.innerHTML = "";
                return;
            }
            slot.innerHTML = `<div class="join-box" style="margin-top:0.8rem;">
        <label class="form-label">پیام برای صاحب پروژه (اختیاری)</label>
        <div class="d-flex gap-2 flex-wrap">
          <input type="text" class="form-control" style="flex:1;min-width:200px;" maxlength="500" data-role="msg" placeholder="مثلاً: با React آشنام و می‌تونم کمک کنم">
          <button type="button" class="btn btn-accent" data-act="join-send">ارسال درخواست</button>
          <button type="button" class="btn btn-line" data-act="slot-close">انصراف</button>
        </div></div>`;
            slot.querySelector("[data-role='msg']").focus();
            return;
        }
        if (act === "slot-close") {
            slot.innerHTML = "";
            return;
        }
        if (act === "join-send") {
            const msg = slot.querySelector("[data-role='msg']").value.trim();
            btn.disabled = true;
            try {
                await CodeMateApi.post(`/teams/${teamId}/join-requests`, { teamId, message: msg || null }, { auth: true });
                await reload(`درخواست عضویت در «${teamName}» ارسال شد. منتظر تصمیم صاحب پروژه باش.`);
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }

        // --- فقط صاحب پروژه
        if (act === "team-edit") {
            if (slot.querySelector(".team-edit")) {
                slot.innerHTML = "";
                return;
            }
            slot.innerHTML = `<div class="team-edit" style="margin-top:0.8rem;">
        <div class="row g-2 align-items-end">
          <div class="col-md-4"><label class="form-label">نام تیم</label>
            <input type="text" class="form-control" maxlength="100" data-role="name" value="${escapeHtml(panel.dataset.name)}"></div>
          <div class="col-md-5"><label class="form-label">توضیحات</label>
            <input type="text" class="form-control" maxlength="500" data-role="desc" value="${escapeHtml(panel.dataset.desc)}"></div>
          <div class="col-md-3 d-flex gap-2">
            <button type="button" class="btn btn-accent flex-fill" data-act="team-save">ذخیره</button>
            <button type="button" class="btn btn-line" data-act="slot-close">انصراف</button>
          </div>
        </div></div>`;
            return;
        }
        if (act === "team-save") {
            const name = slot.querySelector("[data-role='name']").value.trim();
            const description = slot.querySelector("[data-role='desc']").value.trim();
            if (!name || name.length > 100) {
                CodeMateForm.showBanner(banner, "نام تیم اجباریه و نباید بیشتر از ۱۰۰ کاراکتر باشه.");
                return;
            }
            btn.disabled = true;
            try {
                await CodeMateApi.put("/teams", { id: teamId, name, description: description || null }, { auth: true });
                await reload("تیم ویرایش شد.");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }
        if (act === "team-delete") {
            if (!window.confirm(`تیم «${teamName}» حذف بشه؟`)) return;
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/teams/${teamId}`, { auth: true });
                await reload(`تیم «${teamName}» حذف شد.`);
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }
        if (act === "req-accept" || act === "req-reject") {
            const li = btn.closest("li");
            const requestId = li.dataset.request;
            const name = li.dataset.name;
            const accept = act === "req-accept";
            btn.disabled = true;
            try {
                await CodeMateApi.patch(
                    `/teams/${teamId}/join-requests/${requestId}/${accept ? "accept" : "reject"}`,
                    undefined,
                    { auth: true }
                );
                await reload(accept ? `${name} به «${teamName}» اضافه شد.` : `درخواست ${name} رد شد.`);
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
            return;
        }
        if (act === "member-remove") {
            const li = btn.closest("li");
            const { user, name } = li.dataset;
            if (!window.confirm(`${name} از تیم «${teamName}» حذف بشه؟ تسک‌های Assign‌شده به اون می‌مونن.`)) return;
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/teams/${teamId}/members/${user}`, { auth: true });
                await reload(`${name} از تیم حذف شد.`);
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(banner, errText(err));
            }
        }
    }

    init();
})();
