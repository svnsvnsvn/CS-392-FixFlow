function initializeLiveSearch(textBoxId, resultsDivId, hiddenIdFieldId, endpointUrl, getExtraParams) {
    const textBox = document.getElementById(textBoxId);
    const resultsDiv = document.getElementById(resultsDivId);
    const hiddenIdField = document.getElementById(hiddenIdFieldId);

    


    if (!textBox || !resultsDiv || !hiddenIdField) {
        console.error("Live search initialization failed: missing element.");
        return;
    }

    let debounceTimer = null;

    function clearResults() {
        resultsDiv.innerHTML = "";
        resultsDiv.style.display = "none";
    }

    function showResults(items) {
        resultsDiv.innerHTML = "";

        if (!items || items.length === 0) {
            clearResults();
            return;
        }

        for (const item of items) {
            const row = document.createElement("div");
            row.classList.add("typeahead-item");

            row.textContent = `${item.lName}, ${item.fName} (${item.role})`;
            row.dataset.value = item.userId;

            row.addEventListener("click", function () {
                textBox.value = row.textContent;
                hiddenIdField.value = row.dataset.value;
                clearResults();
            });

            resultsDiv.appendChild(row);
        }

        resultsDiv.style.display = "block";
    }

    async function runSearch() {
        const term = textBox.value.trim();

        hiddenIdField.value = "";

        if (term.length < 3) {
            clearResults();
            return;
        }

        const params = new URLSearchParams();
        params.append("term", term);

        if (typeof getExtraParams === "function") {
            const extraParams = getExtraParams();
            if (extraParams) {
                for (const [key, value] of Object.entries(extraParams)) {
                    if (value !== null && value !== undefined && value !== "") {
                        params.append(key, value);
                    }
                }
            }
        }

        try {
            const response = await fetch(`${endpointUrl}&${params.toString()}`);

            if (!response.ok) {
                console.error("Live search failed:", response.status);
                clearResults();
                return;
            }
            const items = await response.json();
            console.log(items);
            showResults(items);
        } catch (error) {
            console.error("Live search error:", error);
            clearResults();
        }
    }
    textBox.addEventListener("input", function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(runSearch, 250);
    });

    textBox.addEventListener("blur", function () {
        setTimeout(clearResults, 150);
    });

    textBox.addEventListener("focus", function () {
        if (resultsDiv.innerHTML.trim() !== "") {
            resultsDiv.style.display = "block";
        }
    });
}