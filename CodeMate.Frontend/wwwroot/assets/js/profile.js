        (() => {
        const content = CodeMateShell.mount({
            active: "profile",
            eyebrow: "حساب کاربری",
            title: "پروفایل و مهارت‌ها",
        });

        const LEVELS = [
            { value: 1, label: "مبتدی" },
            { value: 2, label: "متوسط" },
            { value: 3, label: "پیشرفته" },
            { value: 4, label: "خبره" },
        ];

        content.innerHTML = `
    <div class="panel">
      <h3>اطلاعات تو</h3>
      <div id="profile-banner" class="banner banner-error"></div>
      <form id="profile-form">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">نام کاربری</label>
            <input type="text" class="form-control" id="userName" disabled>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">ایمیل</label>
            <input type="text" class="form-control" id="email" disabled>
          </div>
        </div>
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label" for="fullName">نام کامل</label>
            <input type="text" class="form-control" id="fullName">
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label" for="phoneNumber">شماره تماس</label>
            <input type="tel" class="form-control" id="phoneNumber">
          </div>
        </div>
        <div class="mb-3">
          <label class="form-label" for="bio">بیوگرافی</label>
          <textarea class="form-control" id="bio" rows="3"></textarea>
        </div>
        <button type="submit" class="btn btn-accent" id="save-btn">ذخیره تغییرات</button>
      </form>
    </div>

    <div class="panel">
      <h3>مهارت‌ها</h3>
      <p class="panel-desc">توی کاتالوگ مهارت‌ها جستجو کن و مواردی که بلدی رو اضافه کن — پروژه‌ها بر همین اساس باهات مچ می‌شن.</p>
      <div id="skills-banner" class="banner banner-error"></div>

      <div id="skill-chips" style="margin-bottom:1.1rem;"><div class="skeleton" style="height:30px;width:220px;"></div></div>

      <div class="row g-2 align-items-start">
        <div class="col-md-5" style="position:relative;">
          <label class="form-label" for="skill-search">مهارت</label>
          <input type="text" class="form-control" id="skill-search" placeholder="مثلاً C#, React…" autocomplete="off">
          <div class="skill-search-results" id="skill-results"></div>
        </div>
        <div class="col-md-3">
          <label class="form-label" for="skill-level">سطح</label>
          <select class="form-control" id="skill-level">
            ${LEVELS.map((l) => `<option value="${l.value}">${l.label}</option>`).join("")}
          </select>
        </div>
        <div class="col-md-2">
          <label class="form-label" for="skill-years">سال تجربه</label>
          <input type="number" min="0" class="form-control" id="skill-years" placeholder="0">
        </div>
        <div class="col-md-2 d-flex align-items-end">
          <button type="button" class="btn btn-line w-100" id="add-skill-btn" disabled>افزودن</button>
        </div>
      </div>
    </div>
  `;

        const profileBanner = document.getElementById("profile-banner");
        const skillsBanner = document.getElementById("skills-banner");
        let selectedSkill = null;

        async function loadProfile() {
            try {
                const p = await CodeMateApi.get("/users/me", { auth: true });
                document.getElementById("userName").value = p.userName || "";
                document.getElementById("email").value = p.email || "";
                document.getElementById("fullName").value = p.fullName || "";
                document.getElementById("phoneNumber").value = p.phoneNumber || "";
                document.getElementById("bio").value = p.bio || "";
            } catch (err) {
                CodeMateForm.showBanner(profileBanner, err.message || "پروفایل لود نشد.");
            }
        }

        document.getElementById("profile-form").addEventListener("submit", async (e) => {
            e.preventDefault();
            CodeMateForm.hideBanner(profileBanner);
            const saveBtn = document.getElementById("save-btn");
            CodeMateForm.setLoading(saveBtn, true, "در حال ذخیره…");
            try {
                await CodeMateApi.put(
                    "/users/me",
                    {
                        fullName: document.getElementById("fullName").value.trim() || null,
                        phoneNumber: document.getElementById("phoneNumber").value.trim() || null,
                        bio: document.getElementById("bio").value.trim() || null,
                    },
                    { auth: true }
                );
                CodeMateForm.showBanner(profileBanner, "ذخیره شد.", "ok");
            } catch (err) {
                CodeMateForm.showBanner(profileBanner, err.message || "ذخیره نشد.");
            } finally {
                CodeMateForm.setLoading(saveBtn, false);
            }
        });

        async function loadMySkills() {
            try {
                const skills = await CodeMateApi.get("/skills/me", { auth: true });
                renderChips(skills);
            } catch (err) {
                document.getElementById("skill-chips").innerHTML = "";
                CodeMateForm.showBanner(skillsBanner, err.message || "مهارت‌ها لود نشدن.");
            }
        }

        function renderChips(skills) {
            const holder = document.getElementById("skill-chips");
            if (!skills.length) {
                holder.innerHTML = `<div class="empty-state">هنوز مهارتی اضافه نکردی — از پایین جستجو کن.</div>`;
                return;
            }
            holder.innerHTML = skills
                .map(
                    (s) => `<span class="skill-chip" data-id="${s.id}">
          ${escapeHtml(s.name)}
          <button type="button" aria-label="حذف ${escapeHtml(s.name)}" data-remove="${s.id}">&times;</button>
        </span>`
                )
                .join("");

            holder.querySelectorAll("[data-remove]").forEach((btn) => {
                btn.addEventListener("click", () => removeSkill(btn.dataset.remove));
            });
        }

        async function removeSkill(skillId) {
            CodeMateForm.hideBanner(skillsBanner);
            try {
                await CodeMateApi.del(`/skills/me/${skillId}`, { auth: true });
                loadMySkills();
            } catch (err) {
                CodeMateForm.showBanner(skillsBanner, err.message || "حذف نشد.");
            }
        }

        let searchTimer = null;
        const searchInput = document.getElementById("skill-search");
        const resultsBox = document.getElementById("skill-results");
        const addBtn = document.getElementById("add-skill-btn");

        searchInput.addEventListener("input", () => {
            selectedSkill = null;
            addBtn.disabled = true;
            clearTimeout(searchTimer);
            const term = searchInput.value.trim();
            if (!term) {
                resultsBox.className = "skill-search-results";
                resultsBox.innerHTML = "";
                return;
            }
            searchTimer = setTimeout(() => runSearch(term), 250);
        });

        async function runSearch(term) {
            try {
                const results = await CodeMateApi.get(`/skills?search=${encodeURIComponent(term)}`);
                if (!results.length) {
                    resultsBox.innerHTML = `<div class="opt" style="color:var(--muted);">مهارتی پیدا نشد.</div>`;
                    resultsBox.className = "skill-search-results show";
                    return;
                }
                resultsBox.innerHTML = results
                    .map((s) => `<div class="opt" data-id="${s.id}" data-name="${escapeHtml(s.name)}">${escapeHtml(s.name)}</div>`)
                    .join("");
                resultsBox.className = "skill-search-results show";
                resultsBox.querySelectorAll(".opt[data-id]").forEach((opt) => {
                    opt.addEventListener("click", () => {
                        selectedSkill = { id: opt.dataset.id, name: opt.dataset.name };
                        searchInput.value = opt.dataset.name;
                        resultsBox.className = "skill-search-results";
                        addBtn.disabled = false;
                    });
                });
            } catch (err) {
                resultsBox.innerHTML = `<div class="opt" style="color:var(--danger);">${escapeHtml(err.message || "جستجو ناموفق بود.")}</div>`;
                resultsBox.className = "skill-search-results show";
            }
        }

        addBtn.addEventListener("click", async () => {
            if (!selectedSkill) return;
            CodeMateForm.hideBanner(skillsBanner);
            CodeMateForm.setLoading(addBtn, true, "در حال افزودن…");
            try {
                const years = document.getElementById("skill-years").value;
                await CodeMateApi.post(
                    "/skills/me",
                    {
                        skillId: selectedSkill.id,
                        level: Number(document.getElementById("skill-level").value),
                        yearsOfExperience: years ? Number(years) : null,
                    },
                    { auth: true }
                );
                searchInput.value = "";
                document.getElementById("skill-years").value = "";
                selectedSkill = null;
                loadMySkills();
            } catch (err) {
                CodeMateForm.showBanner(skillsBanner, err.message || "افزودن مهارت ناموفق بود.");
            } finally {
                CodeMateForm.setLoading(addBtn, false);
            }
        });

        document.addEventListener("click", (e) => {
            if (!e.target.closest("#skill-search") && !e.target.closest("#skill-results")) {
                resultsBox.className = "skill-search-results";
            }
        });

        function escapeHtml(str) {
            const div = document.createElement("div");
            div.textContent = String(str);
            return div.innerHTML;
        }

        loadProfile();
        loadMySkills();
    })();
