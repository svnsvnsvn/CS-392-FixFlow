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
        console.log(data);

        updateUserSettings(data);
    });


}

function updateUserSettings(_data)
{
    if (_data.role === "Pending")
    {
        document.querySelectorAll('input[name="SelectedUserRole"]').forEach(r => r.checked = false);
    }
    else
    {
        document.querySelector(`input[name="SelectedUserRole"][value="${_data.role}"]`).checked = true;
    }
    
    document.getElementById("ResetPassOnLogin").checked = _data.resetPassOnLogin;
}
