﻿
function onContinueAnywayClicked(){ 
    localStorage.setItem("continueAnywayClicked", true); 
    hideOverlay();
}

function hideOverlay(){
    document.getElementById("overlay").style.display = "none";
    document.getElementById("page-container").style.display = "block";
}

function showOverlay(){
    document.getElementById("overlay").style.display = "block";
    document.getElementById("page-container").style.display = "none";
}

// supportedBrowserRegEx is defined in supportedBrowserRegEx.js.
// This file is generated by a script defined in the package.json
// run "yarn build" to generate it
if(typeof supportedBrowserRegEx === 'function'){
    if(!supportedBrowserRegEx.test(navigator.userAgent) && !localStorage.continueAnywayClicked){
        showOverlay();
        if(navigator.userAgent.includes("ios")){
             document.getElementById("edgeLink").style.display = "none";
        }
        else
        {
            document.getElementById("safariLink").style.display = "none";
        }
        document.getElementById("continueAnywayLink").onclick = onContinueAnywayClicked;
    }
    else
    {
        hideOverlay();
    }
}
else
{
    console.error("function supportedBrowserRegEx is not defined");
}
