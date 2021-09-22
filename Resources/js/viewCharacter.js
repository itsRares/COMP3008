var char_level, char_race, char_class, racial_total = 0;
var char_list = document.querySelector(".character_list");
var name_input = document.querySelector(".input_name");
var search_character = document.querySelector(".search_character");
var delete_character = document.querySelector(".character_delete");
var error = document.querySelector(".truemarble_error p");

window.addEventListener("load", init);
search_character.addEventListener("click", searchCharacter);
delete_character.addEventListener("click", deleteCharacter);

function init() 
{
    var cookieName = document.cookie.split('=')[1];
    loadCharacter(cookieName);
}

function searchCharacter()
{
    var searchName = document.querySelector(".input_name").value;
    document.querySelector(".i_spell").innerHTML = "Loading...";
    document.querySelector(".i_hit").innerHTML = "Loading...";
    document.querySelector(".i_ability").innerHTML = "Loading...";
    loadCharacter(searchName);
}

function loadCharacter(user)
{
    error.innerHTML = "";
    char_req = null;
    char_req = new XMLHttpRequest();
    char_req.open("GET", "../api/searchCharacter/"+user, true);
    char_req.setRequestHeader("Response-type", "application/json");
    char_req.onreadystatechange = onLoadCharacterComplete;
    char_req.send();
}

function onLoadCharacterComplete()
{
    if(char_req.readyState == 4)
    {
        console.log(char_req.status);
        if(char_req.status == 200)
        {
            var res = (JSON.parse(char_req.responseText));
            document.querySelector(".i_name").innerHTML = res.name;
            document.querySelector(".i_age").innerHTML = res.age;
            document.querySelector(".i_gender").innerHTML = res.gender;
            document.querySelector(".character_biography").innerHTML = res.biography;
            document.querySelector(".i_level").innerHTML = res.level;
            document.querySelector(".i_race").innerHTML = res.race;
            document.querySelector(".i_class").innerHTML = res.class;
            char_ability_score = res.constitution + res.dexterity + res.strength + res.charisma + res.inteligence + res.wisdom
            char_level = res.level;
            char_race = res.race;
            char_class = res.class;
            loadAPIinfo();
        } else {
            error.innerHTML = (JSON.parse(char_req.responseText));
        }
    }
}

function deleteCharacter() 
{
    error.innerHTML = "";
    delete_req = null;
    delete_req = new XMLHttpRequest();
    delete_req.open("POST", "../api/deleteCharacter", true);
    delete_req.setRequestHeader("Content-type", "application/json");
    delete_req.onreadystatechange = onDeleteCharacterComplete;
    var delChar_msg = {name: document.querySelector(".i_name").innerText};
    delete_req.send(JSON.stringify(delChar_msg));
}

function onDeleteCharacterComplete()
{
    if(delete_req.readyState == 4)
    {
        if(delete_req.status == 200 || delete_req.status == 204)
        {
            window.location.href = "/";
        } else {
            error.innerHTML = (JSON.parse(char_req.responseText));
        }
    }
}

function loadAPIinfo()
{
    api_req = null;
    api_req = new XMLHttpRequest();
    api_req.open("GET", "../api/dnd5eapi/"+char_race+"/"+char_class, true);
    api_req.setRequestHeader("Content-type", "application/json");
    api_req.onreadystatechange = onloadAPIComplete;
    api_req.send();
}

function onloadAPIComplete()
{
    if(api_req.readyState == 4)
    {
        if(api_req.status == 200)
        {
            var response = (JSON.parse(api_req.responseText));
            var class_info = response.class_info;
            var race_info = response.race_info;

            document.querySelector(".i_ability").innerHTML = char_ability_score + race_info.racial_total;
            document.querySelector(".i_spell").innerHTML = class_info.spellcaster;
            document.querySelector(".i_hit").innerHTML = (char_level * class_info.hit_die) + race_info.constitution_score;
        } else {
            error.innerHTML = (JSON.parse(api_req.responseText));
        }
    }
}

name_input.addEventListener("keyup", function(event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        search_character.click();
    }
});
