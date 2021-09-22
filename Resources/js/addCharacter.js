//Varibales
var addCharButton = document.querySelector(".addCharacter_submit");
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
addCharButton.addEventListener("click", addCharacter);

function init() 
{
    //Load races and classes list
    loadRCList();
}

//Add character async call
function addCharacter() {
    error.innerHTML = "";
    add_req = null;
    add_req = new XMLHttpRequest();
    add_req.open("POST", "../api/addCharacter", true);
    add_req.setRequestHeader("Content-type", "application/json");
    add_req.onreadystatechange = onAddCharacterComplete;
    var addChar_msg = { name: m_name.value, age: m_age.value, gender: m_gender.value, biography: m_bio.value, level: m_level.value, race: m_race.value, class: m_class.value, constitution: m_constitution.value, dexterity: m_dexterity.value, strength: m_strength.value, charisma: m_charisma.value, inteligence: m_inteligence.value, wisdom: m_wisdom.value};
    add_req.send(JSON.stringify(addChar_msg));
}

function onAddCharacterComplete()
{
    if(add_req.readyState == 4)
    {
        if(add_req.status == 200 || add_req.status == 204)
        {
            //if a success redirect to home page
            window.location.href = "/";
        } else {
            error.innerHTML = (JSON.parse(add_req.responseText));
        }
    }
}

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

            //Dynamically place race options in select tag
            for (let i = 0; i < class_list.length; i++) {
              option = document.createElement('option');
              option.text = class_list[i].name;
              option.value = class_list[i].name;
              m_class.add(option);
            }
        } else {
            error.innerHTML = (JSON.parse(RCList_req.responseText));
        }
    }
}


