
function InitalizeUserSearch(searchBoxId, resultsBoxId, url)
{
    const inputBox = document.getElementById(searchBoxId)
    const resultsBox = document.getElementById(resultsBoxId)
    
    let debounceTimer;

    //console.log(inputBox);
    //console.log(resultsBox);

    inputBox.addEventListener("input", function () {
        const term = this.value;
        if (term.length > 2) {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(async () => {
                const response = await fetch(url + "&term=" + encodeURIComponent(term));
                const data = await response.json();
                //console.log(data);

                updateSearchResults(resultsBox, data);
            }, 300);
        }
    });
}

function updateSearchResults(_resultsBox, _results) {
    _resultsBox.replaceChildren();
    _results.forEach(user => {
        const option = document.createElement("option")
        option.value = user.userId;
        option.title = user.role;
        option.textContent = user.lName + ", " + user.fName;

        _resultsBox.appendChild(option);
    });
}