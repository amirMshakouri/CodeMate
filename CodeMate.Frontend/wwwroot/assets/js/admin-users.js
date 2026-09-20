(() => {
    // فقط برای ادمین. کنترل اصلی دسترسی سمت بک‌اند انجام می‌شه؛ این برای راحتی UIه.
    if (!CodeMateApi.isAdmin()) {
        window.location.href = "dashboard.html";
        return;
    }

    const PAGE_SIZE = 10;
    const myId = ((CodeMateApi.getClaims() || {}).id || "").toLowerCase();

    const ROLE = {
        1: { label: "کاربر", cls: "status-draft" },
        2: { label: "ادمین", cls: "status-done" },
    };

    const content = CodeMateShell.mount({
        active: "admin-users",
        eyebrow: "مدیریت",
        title: "مدیریت کاربران",
    });

    content.innerHTML = `
    <div class="panel">
      <h3>کاربران</h3>
      <p class="panel-desc">جستجو، غیرفعال/فعال‌کردن حساب و ارتقا به ادمین.</p>

      <form id="filter-form" class="row g-2 align-items-end" novalidate>
        <div class="col-md-5">
          <label class="form-label" for="f-term">نام کاربری، ایمیل یا نام کامل</label>
          <input type="text" class="form-control" id="f-term">
        </div>
        <div class="col-md-2">
          <label class="form-label" for="f-role">نقش</label>
          <select class="form-control" id="f-role">
            <option value="">همه</option>
            <option value="1">کاربر</option>
            <option value="2">ادمین</option>
          </select>
        </div>
        <div class="col-md-2">
          <label class="form-label" for="f-active">وضعیت</label>
          <select class="form-control" id="f-active">
            <option value="">همه</option>
            <option value="true">فعال</option>
            <option value="false">غیرفعال</option>
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
    const state = { page: 1, term: "", role: "", active: "" };

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
        if (state.term) params.set("SearchTerm", state.term);
        if (state.role) params.set("Role", state.role);
        if (state.active) params.set("IsActive", state.active);

        try {
            const data = await CodeMateApi.get(`/admin/users?${params.toString()}`, { auth: true });
            renderTable(data.items || []);
            renderPager(data);
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(banner, errText(err));
        }
    }

    function renderTable(users) {
        if (!users.length) {
            holder.innerHTML = `<div class="empty-state">کاربری با این فیلترها پیدا نشد.</div>`;
            return;
        }
        holder.innerHTML = `
      <table class="data-table">
        <thead>
          <tr>
            <th>کاربر</th><th>ایمیل</th><th>نقش</th><th>وضعیت</th><th>عضویت</th><th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          ${users.map(rowHtml).join("")}
        </tbody>
      </table>`;
    }

    function rowHtml(u) {
        const role = ROLE[u.role] || { label: String(u.role), cls: "status-draft" };
        const isMe = String(u.id).toLowerCase() === myId;
        const toggle = u.isActive
            ? `<button type="button" class="btn btn-line btn-sm-line" style="color:var(--danger);" data-act="deactivate" ${isMe ? 'disabled title="نمی‌تونی خودت رو غیرفعال کنی"' : ""}>غیرفعال‌کردن</button>`
            : `<button type="button" class="btn btn-line btn-sm-line" data-act="activate">فعال‌کردن</button>`;
        const promote =
            u.role === 1
                ? `<button type="button" class="btn btn-line btn-sm-line" data-act="promote">ارتقا به ادمین</button>`
                : "";
        return `
      <tr data-id="${escapeHtml(u.id)}" data-name="${escapeHtml(u.userName)}">
        <td>
          <div style="font-weight:600;">${escapeHtml(u.userName)}${isMe ? ' <span class="mono" style="color:var(--muted);">(تو)</span>' : ""}</div>
          <div style="color:var(--muted);font-size:0.8rem;">${escapeHtml(u.fullName || "")}</div>
        </td>
        <td style="direction:ltr;text-align:right;">${escapeHtml(u.email)}</td>
        <td><span class="status-badge ${role.cls}">${role.label}</span></td>
        <td><span class="status-badge ${u.isActive ? "status-active" : "status-off"}">${u.isActive ? "فعال" : "غیرفعال"}</span></td>
        <td>${fmtDate(u.createdAt)}</td>
        <td class="text-nowrap">
          <button type="button" class="btn btn-line btn-sm-line" data-act="details">جزئیات</button>
          ${toggle}
          ${promote}
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
      <span>صفحه ${state.page} از ${totalPages} — ${data.totalCount} کاربر</span>
      <button type="button" class="btn btn-line" id="prev-btn" ${state.page <= 1 ? "disabled" : ""}>قبلی</button>`;
        document.getElementById("prev-btn").addEventListener("click", () => { state.page -= 1; load(); });
        document.getElementById("next-btn").addEventListener("click", () => { state.page += 1; load(); });
    }

    // ---- جزئیات، فعال/غیرفعال، ارتقا
    holder.addEventListener("click", async (e) => {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const tr = btn.closest("tr");
        const id = tr.dataset.id;
        const name = tr.dataset.name;
        const act = btn.dataset.act;

        if (act === "details") {
            const next = tr.nextElementSibling;
            if (next && next.classList.contains("details-row")) {
                next.remove();
                return;
            }
            btn.disabled = true;
            try {
                const d = await CodeMateApi.get(`/admin/users/${id}`, { auth: true });
                const row = document.createElement("tr");
                row.className = "details-row";
                row.innerHTML = `<td colspan="6">
          <div><strong>شماره تماس:</strong> <span style="direction:ltr;display:inline-block;">${escapeHtml(d.phoneNumber || "—")}</span></div>
          <div><strong>بیوگرافی:</strong> ${escapeHtml(d.bio || "—")}</div>
        </td>`;
                tr.after(row);
            } catch (err) {
                CodeMateForm.showBanner(banner, errText(err));
            } finally {
                btn.disabled = false;
            }
            return;
        }

        const config = {
            deactivate: {
                confirm: `حساب «${name}» غیرفعال بشه؟ دیگه نمی‌تونه وارد بشه.`,
                done: `حساب «${name}» غیرفعال شد.`,
            },
            activate: { confirm: null, done: `حساب «${name}» فعال شد.` },
            promote: {
                confirm: `«${name}» به ادمین ارتقا پیدا کنه؟ این کار از همین پنل برگشت‌پذیر نیست.`,
                done: `«${name}» حالا ادمینه.`,
            },
        }[act];
        if (!config) return;
        if (config.confirm && !window.confirm(config.confirm)) return;

        CodeMateForm.hideBanner(banner);
        btn.disabled = true;
        try {
            await CodeMateApi.patch(`/admin/users/${id}/${act}`, undefined, { auth: true });
            await load();
            CodeMateForm.showBanner(banner, config.done, "ok");
        } catch (err) {
            btn.disabled = false;
            CodeMateForm.showBanner(banner, errText(err));
        }
    });

    // ---- فیلتر
    document.getElementById("filter-form").addEventListener("submit", (e) => {
        e.preventDefault();
        state.term = document.getElementById("f-term").value.trim();
        state.role = document.getElementById("f-role").value;
        state.active = document.getElementById("f-active").value;
        state.page = 1;
        load();
    });
    document.getElementById("reset-btn").addEventListener("click", () => {
        document.getElementById("f-term").value = "";
        document.getElementById("f-role").value = "";
        document.getElementById("f-active").value = "";
        state.term = state.role = state.active = "";
        state.page = 1;
        load();
    });

    load();
})();
