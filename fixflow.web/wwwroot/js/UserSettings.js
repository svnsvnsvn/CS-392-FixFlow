function InitalizeUserSettingsFetcher(selectorListId, url)
{
    const selectorList = document.getElementById(selectorListId)

    //console.log(selectorList)

    selectorList.addEventListener("change", async function () {
        const selectedUser = this.value;

        //console.log("Change Hit");
        //console.log(selectedUser);
        const response = await fetch(url + "&selectedUser=" + encodeURIComponent(selectedUser));
        const data = await response.json();
        updateUserSettings(data);
    });


}

function updateUserSettings(_data)
{
    function setInput(id, val) {
        const el = document.getElementById(id);
        if (!el) return;
        if (val === null || val === undefined) {
            el.value = "";
            return;
        }
        el.value = String(val);
    }

    setInput("EditFName", _data.fName);
    setInput("EditLName", _data.lName);
    setInput("EditUserName", _data.userName);
    setInput("EditEmail", _data.email);
    setInput("EditPhone", _data.phoneNumber);
    setInput("EditLocationCode", _data.locationCode);
    setInput("EditUnit", _data.unit);

    if (_data.role === "Pending")
    {
        document.querySelectorAll('input[name="SelectedUserRole"]').forEach(r => r.checked = false);
    }
    else
    {
        const radio = document.querySelector(`input[name="SelectedUserRole"][value="${_data.role}"]`);
        if (radio) radio.checked = true;
    }

    const resetCb = document.getElementById("ResetPassOnLogin");
    if (resetCb) resetCb.checked = !!_data.resetPassOnLogin;
}
