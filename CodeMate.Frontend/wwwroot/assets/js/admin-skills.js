(() => {
    // این صفحه فقط برای ادمینه. کنترل اصلی سمت بک‌اند انجام می‌شه؛ این فقط برای راحتی UIه.
    if (!CodeMateApi.isAdmin()) {
        window.location.href = "dashboard.html";
        return;
    }

    const MAX_LEN = 100;

    const STARTER_SKILLS = [
        "C#", ".NET", "ASP.NET Core", "Entity Framework", "SQL Server",
        "JavaScript", "TypeScript", "HTML", "CSS", "React", "Vue.js", "Angular",
        "Node.js", "Python", "Django", "Java", "Spring Boot",
        "REST API", "Git", "Docker", "UI/UX Design", "Figma",
        "Unit Testing", "Agile / Scrum",
    ];

    const content = CodeMateShell.mount({
        active: "admin-skills",
        eyebrow: "مدیریت",
        title: "کاتالوگ مهارت‌ها",
    });

    content.innerHTML = `
    <div class="panel">
      <h3>افزودن مهارت</h3>
      <p class="panel-desc">کاربرها فقط از مهارت‌هایی که اینجا تعریف شده به پروفایلشون اضافه می‌کنن.</p>
      <div id="add-banner" class="banner"></div>
      <form id="add-form" class="row g-2 align-items-end" novalidate>
        <div class="col-md-9">
          <label class="form-label" for="new-skill">نام مهارت</label>
          <input type="text" class="form-control" id="new-skill" maxlength="${MAX_LEN}" placeholder="مثلاً React">
        </div>
        <div class="col-md-3">
          <button type="submit" class="btn btn-accent w-100" id="add-btn">افزودن</button>
        </div>
      </form>

      <hr style="border-color:var(--line);margin:1.4rem 0;">

      <label class="form-label" for="bulk-input">افزودن دسته‌ای (هر مهارت توی یه خط)</label>
      <textarea class="form-control" id="bulk-input" rows="5" placeholder="Python&#10;Docker&#10;Figma"></textarea>
      <div class="d-flex gap-2 mt-2 flex-wrap">
        <button type="button" class="btn btn-accent" id="bulk-btn">افزودن همه</button>
        <button type="button" class="btn btn-line" id="fill-starter-btn">پر کردن با لیست پیشنهادی</button>
      </div>
      <div id="bulk-result" style="margin-top:1rem;font-size:0.88rem;"></div>
    </div>

    <div class="panel">
      <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
        <div>
          <h3>مهارت‌های موجود</h3>
          <p class="panel-desc" id="count-text" style="margin-bottom:0;">…</p>
        </div>
      </div>
      <form id="search-form" class="row g-2 align-items-end" style="margin-top:1rem;" novalidate>
        <div class="col-md-9">
          <input type="text" class="form-control" id="search-input" placeholder="جستجو در مهارت‌ها">
        </div>
        <div class="col-md-3 d-flex gap-2">
          <button type="submit" class="btn btn-line flex-fill">جستجو</button>
          <button type="button" class="btn btn-line" id="search-reset-btn">پاک‌کردن</button>
        </div>
      </form>
      <div id="list-banner" class="banner" style="margin-top:1rem;"></div>
      <div id="list-holder" style="margin-top:1rem;"></div>
    </div>
  `;

    const addBanner = document.getElementById("add-banner");
    const listBanner = document.getElementById("list-banner");
    const holder = document.getElementById("list-holder");
    const bulkResult = document.getElementById("bulk-result");
    const countText = document.getElementById("count-text");

    let searchTerm = "";

    function escapeHtml(str) {
        const div = document.createElement("div");
        div.textContent = String(str);
        return div.innerHTML;
    }

    // بک‌اند ارور ولیدیشن رو داخل errors می‌ذاره؛ اولین پیام قابل‌فهم‌تره
    function errText(err) {
        if (err && err.errors) {
            const first = Object.values(err.errors).flat()[0];
            if (first) return first;
        }
        return (err && err.message) || "خطای ناشناخته";
    }

    function validName(raw) {
        const name = raw.trim();
        if (!name) return { error: "نام مهارت خالیه." };
        if (name.length > MAX_LEN) return { error: `نام مهارت نباید بیشتر از ${MAX_LEN} کاراکتر باشه.` };
        return { name };
    }

    async function load() {
        CodeMateForm.hideBanner(listBanner);
        holder.innerHTML = `<div class="skeleton" style="height:44px;margin-bottom:8px;"></div><div class="skeleton" style="height:44px;"></div>`;
        try {
            const path = searchTerm ? `/skills?search=${encodeURIComponent(searchTerm)}` : "/skills";
            const skills = await CodeMateApi.get(path, { auth: true });
            render(Array.isArray(skills) ? skills : []);
        } catch (err) {
            holder.innerHTML = "";
            CodeMateForm.showBanner(listBanner, errText(err));
        }
    }

    function rowView(skill) {
        return `
      <span class="project-dot"></span>
      <span class="project-title">${escapeHtml(skill.name)}</span>
      <button type="button" class="btn btn-line" style="padding:0.25rem 0.7rem;font-size:0.82rem;" data-act="edit">ویرایش</button>
      <button type="button" class="btn btn-line" style="padding:0.25rem 0.7rem;font-size:0.82rem;color:var(--danger);" data-act="delete">حذف</button>`;
    }

    function rowEdit(skill) {
        return `
      <input type="text" class="form-control" style="flex:1;" maxlength="${MAX_LEN}" value="${escapeHtml(skill.name)}" data-role="edit-input">
      <button type="button" class="btn btn-accent" style="padding:0.25rem 0.7rem;font-size:0.82rem;" data-act="save">ذخیره</button>
      <button type="button" class="btn btn-line" style="padding:0.25rem 0.7rem;font-size:0.82rem;" data-act="cancel">انصراف</button>`;
    }

    function render(skills) {
        countText.textContent = searchTerm
            ? `${skills.length} نتیجه برای «${searchTerm}»`
            : `${skills.length} مهارت توی کاتالوگ هست`;

        if (!skills.length) {
            holder.innerHTML = `<div class="empty-state">${
                searchTerm ? "مهارتی با این عبارت پیدا نشد." : "کاتالوگ خالیه. از بالا مهارت اضافه کن."
            }</div>`;
            return;
        }

        const sorted = [...skills].sort((a, b) => a.name.localeCompare(b.name));
        holder.innerHTML = `<ul class="project-list is-rich">
      ${sorted
          .map((s) => `<li data-id="${escapeHtml(s.id)}" data-name="${escapeHtml(s.name)}">${rowView(s)}</li>`)
          .join("")}
    </ul>`;
    }

    // ---- افزودن تکی
    document.getElementById("add-form").addEventListener("submit", async (e) => {
        e.preventDefault();
        CodeMateForm.hideBanner(addBanner);
        const input = document.getElementById("new-skill");
        const v = validName(input.value);
        if (v.error) {
            CodeMateForm.showBanner(addBanner, v.error);
            return;
        }
        const btn = document.getElementById("add-btn");
        CodeMateForm.setLoading(btn, true, "در حال افزودن…");
        try {
            await CodeMateApi.post("/admin/skills", { name: v.name }, { auth: true });
            input.value = "";
            CodeMateForm.showBanner(addBanner, `«${v.name}» اضافه شد.`, "ok");
            await load();
        } catch (err) {
            CodeMateForm.showBanner(addBanner, errText(err));
        } finally {
            CodeMateForm.setLoading(btn, false);
        }
    });

    // ---- افزودن دسته‌ای
    document.getElementById("fill-starter-btn").addEventListener("click", () => {
        document.getElementById("bulk-input").value = STARTER_SKILLS.join("\n");
    });

    document.getElementById("bulk-btn").addEventListener("click", async () => {
        const raw = document.getElementById("bulk-input").value;
        const names = [...new Set(raw.split(/\r?\n/).map((n) => n.trim()).filter(Boolean))];
        if (!names.length) {
            bulkResult.innerHTML = `<span style="color:var(--danger);">هیچ نامی وارد نشده.</span>`;
            return;
        }

        const btn = document.getElementById("bulk-btn");
        CodeMateForm.setLoading(btn, true, "در حال افزودن…");
        let added = 0;
        const failed = [];
        for (const name of names) {
            const v = validName(name);
            if (v.error) {
                failed.push(`${name.slice(0, 30)}: ${v.error}`);
                continue;
            }
            try {
                await CodeMateApi.post("/admin/skills", { name: v.name }, { auth: true });
                added++;
            } catch (err) {
                failed.push(`${v.name}: ${errText(err)}`);
            }
        }
        CodeMateForm.setLoading(btn, false);

        bulkResult.innerHTML =
            `<div style="color:var(--accent-dark);font-weight:600;">${added} مهارت اضافه شد.</div>` +
            (failed.length
                ? `<div style="color:var(--danger);margin-top:0.4rem;">${failed.length} مورد اضافه نشد:</div>
           <ul style="margin:0.3rem 0 0;padding-inline-start:1.2rem;color:var(--ink-soft);">
             ${failed.map((f) => `<li>${escapeHtml(f)}</li>`).join("")}
           </ul>`
                : "");
        if (added) {
            document.getElementById("bulk-input").value = failed.length ? document.getElementById("bulk-input").value : "";
            await load();
        }
    });

    // ---- ویرایش / حذف داخل لیست
    holder.addEventListener("click", async (e) => {
        const btn = e.target.closest("button[data-act]");
        if (!btn) return;
        const li = btn.closest("li");
        const skill = { id: li.dataset.id, name: li.dataset.name };
        const act = btn.dataset.act;

        if (act === "edit") {
            li.innerHTML = rowEdit(skill);
            li.querySelector("[data-role='edit-input']").focus();
            return;
        }
        if (act === "cancel") {
            li.innerHTML = rowView(skill);
            return;
        }
        if (act === "save") {
            CodeMateForm.hideBanner(listBanner);
            const v = validName(li.querySelector("[data-role='edit-input']").value);
            if (v.error) {
                CodeMateForm.showBanner(listBanner, v.error);
                return;
            }
            if (v.name === skill.name) {
                li.innerHTML = rowView(skill);
                return;
            }
            btn.disabled = true;
            try {
                await CodeMateApi.put(`/admin/skills/${skill.id}`, { name: v.name }, { auth: true });
                await load();
                CodeMateForm.showBanner(listBanner, "تغییر ذخیره شد.", "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(listBanner, errText(err));
            }
            return;
        }
        if (act === "delete") {
            if (!window.confirm(`مهارت «${skill.name}» حذف بشه؟ از پروفایل کاربرها هم دیده نمی‌شه.`)) return;
            CodeMateForm.hideBanner(listBanner);
            btn.disabled = true;
            try {
                await CodeMateApi.del(`/admin/skills/${skill.id}`, { auth: true });
                await load();
                CodeMateForm.showBanner(listBanner, `«${skill.name}» حذف شد.`, "ok");
            } catch (err) {
                btn.disabled = false;
                CodeMateForm.showBanner(listBanner, errText(err));
            }
        }
    });

    // ---- جستجو
    document.getElementById("search-form").addEventListener("submit", (e) => {
        e.preventDefault();
        searchTerm = document.getElementById("search-input").value.trim();
        load();
    });
    document.getElementById("search-reset-btn").addEventListener("click", () => {
        document.getElementById("search-input").value = "";
        searchTerm = "";
        load();
    });

    load();
})();
