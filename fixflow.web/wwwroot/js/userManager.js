

let userIDs = []        // EMpty array for userID list




document.addEventListener('DOMContentLoaded', function ()
{
    // Create list of all user ID's
    let inputItems = document.querySelectorAll('tr[id^="row-"]');   // Get all table rows with an id that starts with "row-"
 

    for (let i = 0; i < inputItems.length; i++)     // Iterate the TR ID's, and slice off "row-" then push each naked IDs to list
    {
        userIDs.push(inputItems[i].getAttribute("id").slice(4));
    }
    
    console.log(userIDs);

    // Add listeners & set initial visibility

    // Get all filter checkboxes and then add click listeners to check visibility and update page
    var visibilityCheckboxes = document.querySelectorAll('input[name^="filter-"]');
    for (let i = 0; i < visibilityCheckboxes.length; i++)
    {
        visibilityCheckboxes[i].addEventListener('click', function () { updateVisibility() });
    }

    // Add listeneres for each user control
    for (let i = 0; i < userIDs.length; i++)
    {
        // Add Listeners to user access controls
        document.getElementById("role-" + userIDs[i] + "-P").addEventListener('click', function () { rowChange(userIDs[i]) });
        document.getElementById("role-" + userIDs[i] + "-R").addEventListener('click', function () { rowChange(userIDs[i]) });
        document.getElementById("role-" + userIDs[i] + "-E").addEventListener('click', function () { rowChange(userIDs[i]) });
        document.getElementById("role-" + userIDs[i] + "-M").addEventListener('click', function () { rowChange(userIDs[i]) });
        document.getElementById("role-" + userIDs[i] + "-A").addEventListener('click', function () { rowChange(userIDs[i]) });
        document.getElementById("lcko-" + userIDs[i]).addEventListener('click', function () { rowChange(userIDs[i]) });
     
        // Add listeners to user password reset buttons
        document.getElementById("pRst-" + userIDs[i]).addEventListener('click', function () { updateRecord(userIDs[i], true, false, document.querySelector('input[name="role-' + userIDs[i] + '"]:checked').value) });

        // Add listeners to apply buttons and make invisible
        document.getElementById("aply-" + userIDs[i]).addEventListener('click', function () { updateRecord(userIDs[i], false, document.querySelector('input[name="lcko-' + userIDs[i] + '"]').checked, document.querySelector('input[name="role-' + userIDs[i] + '"]:checked').value) });
    }
})



function updateVisibility()
// This function will run when one of the role visibility checkboxes changes.
// Visibility will be update row by row based on checkbox values.
{
    // Get filtermask
    pending = document.getElementById("filter-Pending").checked;
    clerk = document.getElementById("filter-Resident").checked;
    employee = document.getElementById("filter-Employee").checked;
    manager = document.getElementById("filter-Manager").checked;
    admin = document.getElementById("filter-Admin").checked;


    // Iterate each user and apply visibility based on their current role and the boolean filtermask
    for (let i = 0; i < userIDs.length; i++) {
        switch (document.querySelector('input[name="role-' + userIDs[i] + '"]:checked').value)
        {   
            case "Pending":
                {
                    document.getElementById("row-" + userIDs[i]).style.visibility = pending ? "visible" : "collapse";
                    break;
                }
            case "Resident":
                {
                    document.getElementById("row-" + userIDs[i]).style.visibility = clerk ? "visible" : "collapse";
                    break;
                }
            case "Employee":
                {
                    document.getElementById("row-" + userIDs[i]).style.visibility = employee ? "visible" : "collapse";
                    break;
                }
            case "Manager":
                {
                    document.getElementById("row-" + userIDs[i]).style.visibility = manager ? "visible" : "collapse";
                    break;
                }
            case "Admin":
                {
                    document.getElementById("row-" + userIDs[i]).style.visibility = admin ? "visible" : "collapse";
                    break;
                }
        }
    }
}

function rowChange(userID)
// This function will use the userID provided and to bold and apply
// a light grey background to the appropriate table row. The apply
// button will be made visible to save changes.
{
    document.getElementById("row-" + userID).style.backgroundColor = "lightgray";
    document.getElementById("row-" + userID).style.fontWeight = "Bold";
    document.getElementById("aply-" + userID).style.visibility = "visible";
}

function rowChangeSuccess(userID)
// This function will use the userID provided and to bold and apply
// a light grey background to the appropriate table row. The apply
// button will be made visible to save changes.
{
    document.getElementById("row-" + userID).style.backgroundColor = "lightgreen";
    document.getElementById("row-" + userID).style.fontWeight = "Normal"
    document.getElementById("aply-" + userID).style.visibility = "hidden";
}

function rowChangeFail(userID)
// This function will use the userID provided and to bold and apply
// a light grey background to the appropriate table row. The apply
// button will be made visible to save changes.
{
    document.getElementById("row-" + userID).style.backgroundColor = "Orangered";
    document.getElementById("row-" + userID).style.fontWeight = "Bold";
    document.getElementById("aply-" + userID).style.visibility = "visible";
}

async function updateRecord(userID, resetPswd, lockAccount, newRole)
// This function will pass the changes for the provided userID to the back end
// for database update.
// ENSURE resetpswd, lockAccount, newRole are set to desired outcome.
// All items are evaluated so you can reset password, lock account, and change role in one call.
// For example if you dont want to change role, you need to provide current role, if you leave blank it will be downgraded to non-user

{
    //  window.alert("Update Record for User: " + userID)
 
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const result = await fetch('/AdminTools/Index?handler=UpdateData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            UserID: userID,
            ResetPswd: resetPswd,
            LockAccount : lockAccount,
            NewRole : newRole
        })
    });

  
        const text = await result.text();       // Read once
        const response = JSON.parse(text);      // Now parse it
        console.log(response);

        if (response.success) {
            rowChangeSuccess(userID);
        } else {
            rowChangeFail(userID);
        }

        if (response.passwordChanged) {
            alert("Password for User:\n" + userID + "\n\nReset to: P@ssw0rd!");
        }
}

