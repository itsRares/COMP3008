//variables
﻿var editCharButton = document.querySelector(".editCharacter_submit");
var m_name = document.querySelector(".input_name");
var m_age = document.querySelector(".input_age");
var m_gender = document.querySelector(".input_gender");
var m_bio = document.querySelector(".input_bio");
var m_level = document.querySelector(".input_level");
var m_race = document.querySelector(".input_race");
var m_class = document.querySelector(".input_class");
var m_constitution = document.querySelector(".input_constitution");
var m_dexterity = document.querySelector(".input_dexterity");
var m_strength = document.querySelector(".input_strength");
var m_charisma = document.querySelector(".input_charisma");
var m_inteligence = document.querySelector(".input_inteligence");
var m_wisdom = document.querySelector(".input_wisdom");
var error = document.querySelector(".truemarble_error p");

window.addEventListener("load", init);
editCharButton.addEventListener("click", editCharacter);

function init()
{
    //Get the cookie with the character name
    cookieName = document.cookie.split('=')[1];
    m_name.value = cookieName
    //Load the races and classes list
    loadRCList();
    //load the specific character
    loadCharacter(cookieName);
}

//Edit character async call
function editCharacter() {
    error.innerHTML = "";
    edit_req = null;
    edit_req = new XMLHttpRequest();
    edit_req.open("POST", "../api/editCharacter", true);
    edit_req.setRequestHeader("Content-type", "application/json");
    edit_req.onreadystatechange = onEditCharacterComplete;
    var addChar_msg = { name: m_name.value, age: m_age.value, gender: m_gender.value, biography: m_bio.value, level: m_level.value, race: m_race.value, class: m_class.value, constitution: m_constitution.value, dexterity: m_dexterity.value, strength: m_strength.value, charisma: m_charisma.value, inteligence: m_inteligence.value, wisdom: m_wisdom.value};
    edit_req.send(JSON.stringify(addChar_msg));
}

function onEditCharacterComplete()
{
    if(edit_req.readyState == 4)
    {
        if(edit_req.status == 200 || edit_req.status == 204)
        {
            //If a success then redirect to home page
            window.location.href = "/";
        } else {
            //Else throw error
            error.innerHTML = (JSON.parse(edit_req.responseText));
        }
    }
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
        if(char_req.status == 200)
        {
            //Once character is loaded add all the information to the inputs
            var res = (JSON.parse(char_req.responseText));
            m_age.value = res.age;
            m_gender.value = res.gender;
            m_bio.innerHTML = res.biography;
            m_level.value = res.level;
            m_constitution.value = res.constitution;
            m_dexterity.value = res.dexterity;
            m_strength.value = res.strength;
            m_charisma.value = res.charisma;
            m_inteligence.value = res.inteligence;
            m_wisdom.value = res.wisdom
            race_value = res.race;
            class_value = res.class;
        } else {
            //Else throw an error
            error.innerHTML = (JSON.parse(char_req.responseText));
        }
    }
}

//Load the races and classes list
function loadRCList()
{
    RCList_req = null;
    RCList_req = new XMLHttpRequest();
    RCList_req.open("GET", "../api/dnd5eapi/RCList", true);
    RCList_req.setRequestHeader("Content-type", "application/json");
    RCList_req.onreadystatechange = onLoadRCListComplete;
    RCList_req.send();
}

function onLoadRCListComplete()
{
    if(RCList_req.readyState == 4)
    {
        if(RCList_req.status == 200)
        {
            var response = (JSON.parse(RCList_req.responseText));
            var race_list = response.race_list.results;
            var class_list = response.class_list.results;

            //Dynamically place race options in select tag
            for (let i = 0; i < race_list.length; i++) {
              option = document.createElement('option');
              option.text = race_list[i].name;
              option.value = race_list[i].name;
              m_race.add(option);
            }

            //Dynamically place class options in select tag
            for (let i = 0; i < class_list.length; i++) {
              option = document.createElement('option');
              option.text = class_list[i].name;
              option.value = class_list[i].name;
              m_class.add(option);
            }

            //Dynamically update the selected race/class
            document.querySelector('.input_race [value="' +race_value + '"]').selected = true;
            document.querySelector('.input_class [value="' +class_value + '"]').selected = true;
        } else {
            //If an error
            error.innerHTML = (JSON.parse(RCList_req.responseText));
        }
    }
}
