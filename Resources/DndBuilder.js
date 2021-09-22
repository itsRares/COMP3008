var addCharButton = document.querySelector(".addCharacter_submit");
var m_name = document.querySelector(".input_name");
var m_age = document.querySelector(".input_age");
var m_gender = document.querySelector(".input_gender");
var m_bio = document.querySelector(".input_bio");
var m_level = document.querySelector(".input_level");
var m_race = document.querySelector(".input_race");
var m_class = document.querySelector(".input_class");
var error = document.querySelector(".truemarble_error p");

window.addEventListener("load", init);
addCharButton.addEventListener("click", addCharacter);

function init() 
{
    console.log("Loaded");
}

function addCharacter() {
    xy_req = null;
    error.innerHTML = "";
    xy_req = new XMLHttpRequest();
    xy_req.open("POST", "../api/addCharacter", true);
    xy_req.setRequestHeader("Content-type", "application/json");
    xy_req.onreadystatechange = onAddCharacterComplete;
    var addChar_msg = { name: m_name.value, age: m_age.value, gender: m_gender.value, biography: m_bio.value, level: m_level.value, race: m_race.value, class: m_class.value };
    console.log(addChar_msg);
    xy_req.send(JSON.stringify(addChar_msg));
}

function onAddCharacterComplete()
{
    if(xy_req.readyState == 4)
    {
        if(xy_req.status == 200)
        {
            window.location.href = "/";
        } else {
            error.innerHTML = (JSON.parse(xy_req.responseText));
        }
    }
}
