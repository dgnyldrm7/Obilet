document.addEventListener("DOMContentLoaded", function () {
    const originSelect = document.getElementById("originSelect");
    const destinationSelect = document.getElementById("destinationSelect");
    const originName = document.getElementById("originName");
    const destinationName = document.getElementById("destinationName");
    const departureDate = document.getElementById("departureDate");
    const dateDisplay = document.getElementById("dateDisplay");
    const btnSwap = document.getElementById("btnSwap");
    const btnToday = document.getElementById("btnToday");
    const btnTomorrow = document.getElementById("btnTomorrow");
    const form = document.getElementById("searchForm");

    // Spinner ve Buton Elemanları
    const searchBtn = document.getElementById("btnSubmitSearch");
    const searchSpinner = document.getElementById("searchSpinner");
    const btnText = document.getElementById("btnSearchText");

    function formatDateISO(d) {
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    function updateDateLabel(isoDateStr) {
        if (!isoDateStr) return;
        const parts = isoDateStr.split('-');
        const date = new Date(parts[0], parts[1] - 1, parts[2]);
        const options = { day: 'numeric', month: 'long', year: 'numeric', weekday: 'long' };
        dateDisplay.textContent = date.toLocaleDateString('tr-TR', options);

        const todayStr = formatDateISO(new Date());
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const tomorrowStr = formatDateISO(tomorrow);

        btnToday.classList.toggle("active", isoDateStr === todayStr);
        btnTomorrow.classList.toggle("active", isoDateStr === tomorrowStr);
    }

    function syncNames() {
        if (originSelect.selectedIndex >= 0) {
            const optOrigin = originSelect.options[originSelect.selectedIndex];
            if (optOrigin) {
                originName.value = optOrigin.getAttribute("data-name") || optOrigin.text.trim();
            }
        }
        if (destinationSelect.selectedIndex >= 0) {
            const optDest = destinationSelect.options[destinationSelect.selectedIndex];
            if (optDest) {
                destinationName.value = optDest.getAttribute("data-name") || optDest.text.trim();
            }
        }
    }

    // 1. Varsa son aramayı localStorage'dan yükle
    const lastSearch = localStorage.getItem("last_journey_search");
    if (lastSearch) {
        try {
            const parsed = JSON.parse(lastSearch);
            if (parsed.originId) originSelect.value = parsed.originId;
            if (parsed.destinationId) destinationSelect.value = parsed.destinationId;

            const todayStr = formatDateISO(new Date());
            if (parsed.departureDate && parsed.departureDate >= todayStr) {
                departureDate.value = parsed.departureDate;
            }
        } catch (e) {
            console.error("LocalStorage okuma hatası:", e);
        }
    }

    // 2. İlk açılışta ve localStorage yüklemesinden sonra etiketleri ve gizli input'ları eşitle
    updateDateLabel(departureDate.value);
    syncNames();

    // 3. Event Listener'lar
    originSelect.addEventListener("change", syncNames);
    destinationSelect.addEventListener("change", syncNames);

    departureDate.addEventListener("change", function () {
        updateDateLabel(this.value);
    });

    btnSwap.addEventListener("click", function () {
        const tempVal = originSelect.value;
        originSelect.value = destinationSelect.value;
        destinationSelect.value = tempVal;
        syncNames();
    });

    btnToday.addEventListener("click", function () {
        const todayStr = formatDateISO(new Date());
        departureDate.value = todayStr;
        updateDateLabel(todayStr);
    });

    btnTomorrow.addEventListener("click", function () {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const tomorrowStr = formatDateISO(tomorrow);
        departureDate.value = tomorrowStr;
        updateDateLabel(tomorrowStr);
    });

    // 4. Form Gönderimi ve Validasyon
    form.addEventListener("submit", function (e) {
        if (originSelect.value === destinationSelect.value) {
            e.preventDefault();
            alert("Kalkış ve varış noktası aynı olamaz.");
            return;
        }

        const todayStr = formatDateISO(new Date());
        if (departureDate.value < todayStr) {
            e.preventDefault();
            alert("Geçmiş bir tarih seçilemez.");
            return;
        }

        syncNames();

        localStorage.setItem("last_journey_search", JSON.stringify({
            originId: originSelect.value,
            destinationId: destinationSelect.value,
            departureDate: departureDate.value
        }));

        // Validasyonlar geçildi: Spinner'ı aktif et ve butonu kilitle
        if (searchBtn && searchSpinner && btnText) {
            searchSpinner.classList.remove("d-none");
            btnText.textContent = "Seferler Aranıyor...";
            searchBtn.classList.add("disabled", "opacity-75");
        }
    });

    // 5. Geri butonuyla dönüldüğünde butonu sıfırla (BFCache koruması)
    window.addEventListener("pageshow", function () {
        if (searchBtn && searchSpinner && btnText) {
            searchSpinner.classList.add("d-none");
            btnText.textContent = "Bileti Bul";
            searchBtn.classList.remove("disabled", "opacity-75");
        }
    });
});