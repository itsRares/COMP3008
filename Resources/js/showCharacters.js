//Variables
var button;
var char_list = document.querySelector(".character_list");
var error = document.querySelector(".truemarble_error p");

window.addEventListener("load", init);

function init()
{
    //Load all the characters
    loadCharacters();
}

//Template to load character
function characterDisplay(name, level, cclass, race)
{
    var div = '<div class="character_wrap"><div class="character_left"><a data-name="'+name+'" class="char_cookie">';
    div += '<div class="character_level"><p>'+level+'</p></div><div class="character_name_wrap">';
    div += '<p class="character_name">'+name+'</p><p class="character_race">('+race+' / '+cclass+')</p>';
    div += '</div></a></div><div class="character_right">';
    div += '<a><div class="dndbuilder_option character_download" data-name="'+name+'"><i class="fas fa-file-download"></i></div></a>';
    div += '<a data-name="'+name+'" class="char_cookie" href="/editCharacter.html"><div class="dndbuilder_option character_edit"><i class="fas fa-pencil-alt"></i></div></a>';
    div += '</div></div>';
    return div;
}

//Once selected a character set the cookie to the name of the character
function setCharacter()
{
    var name = event.target.closest(".char_cookie").getAttribute('data-name');
    document.cookie = "name="+name;
    window.location.href = "/viewCharacter.html";
}

//Load character async
function loadCharacters()
{
    race_req = null;
    race_req = new XMLHttpRequest();
    race_req.open("GET", "../api/searchAllCharacters", true);
    race_req.setRequestHeader("Response-type", "application/json");
    race_req.onreadystatechange = onLoadRacesComplete;
    race_req.send();
}

function onLoadRacesComplete()
{
    if(race_req.readyState == 4)
    {
        if(race_req.status == 200)
        {
            var res = (JSON.parse(race_req.responseText));
            //For each character append it with template to the list
            for (let i = 1; i <= Object.keys(res).length; i++) {
                char_list.innerHTML = characterDisplay(res[i.toString()].name, res[i.toString()].level, res[i.toString()].class, res[i.toString()].race) + char_list.innerHTML;
            }

            //Add a listening to when Character is selected
            selectCharacter = document.getElementsByClassName("char_cookie");
            for (var i = 0; i < selectCharacter.length; i++) {
                selectCharacter[i].addEventListener('click',setCharacter,false);
            }

            //Add a listener to when character download button is pressed
            download_character = document.getElementsByClassName("character_download");
            for (var i = 0; i < download_character.length; i++) {
                download_character[i].addEventListener("click", downloadCharacter, false);
            }
        } else {
            //else throw an error
            error.innerHTML = (JSON.parse(race_req.responseText));
        }
    }
}

//Download the character
function downloadCharacter()
{
    //Set the element to variable for future use
    button = this;
    button_name = this.getAttribute('data-name');
    //Show loading spinner
    button.innerHTML = "<i class='fas fa-spinner fa-spin'></i>";
    //load character by name
    loadCharacter(button_name);

}

//Load character async
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
            //Global char_res so it can be used later when downloading character
            char_res = (JSON.parse(char_req.responseText));
            //Load the api info
            loadAPIinfo(char_res.race, char_res.class);
        } else {
            error.innerHTML = (JSON.parse(char_req.responseText));
        }
    }
}

//load api info async
function loadAPIinfo(char_race, char_class)
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
            //Global api_info so it can be used later when downloading character
            api_info = (JSON.parse(api_req.responseText));
            //Dynamically set up xml
            characterXML();
        } else {
            error.innerHTML = (JSON.parse(api_req.responseText));
        }
    }
}

function characterXML()
{
    //Character info
    var char_ability_score = char_res.constitution + char_res.dexterity + char_res.strength + char_res.charisma + char_res.inteligence + char_res.wisdom
    var char_level = char_res.level;

    //API Info
    var class_info = api_info.class_info;
    var race_info = api_info.race_info;

    var ability_score = char_ability_score + race_info.racial_total;
    var spellcaster = class_info.spellcaster;
    var hit_points = (char_level * class_info.hit_die) + race_info.constitution_score;

    var charxml = "<character>\n";
        charxml += "<name>"+char_res.name+"</name>";
        charxml += "<age>"+char_res.age+"</age>";
        charxml += "<gender>"+char_res.gender+"</gender>";
        charxml += "<level>"+char_res.level+"</level>";
        charxml += "<biography>"+char_res.biography+"</biography>";
        charxml += "<race>"+char_res.race+"</race>";
        charxml += "<class>"+char_res.class+"</class>";
        charxml += "<spellcaster>"+spellcaster+"</spellcaster>";
        charxml += "<hitpoints>"+hit_points+"</hitpoints>";
        charxml += "<abilityscore>"+ability_score+"</abilityscore>";
        charxml += "<abilityscores>";
        charxml += "<constitution>"+char_res.constitution+"</constitution>";
            charxml += "<dexterity>"+char_res.dexterity+"</dexterity>";
            charxml += "<strength>"+char_res.strength+"</strength>";
            charxml += "<charisma>"+char_res.charisma+"</charisma>";
            charxml += "<inteligence>"+char_res.inteligence+"</inteligence>";
            charxml += "<wisdom>"+char_res.wisdom+"</wisdom>";
        charxml += "</abilityscores>";
    charxml += "</character>";

    //Set file name
    var filename = button_name+".xml";
    var el = document.createElement('a');
    var bb = new Blob([charxml], {type: 'text/plain'});

    el.setAttribute('href', window.URL.createObjectURL(bb));
    el.setAttribute('download', filename);

    //Set values
    el.dataset.downloadurl = ['text/plain', el.download, el.href].join(':');
    el.draggable = true;
    el.classList.add('dragout');

    el.click();
    button.innerHTML = '<i class="fas fa-check"></i>';
}
