function JsCopyAdress() {

    var elementInDataNames = ["Js-PostAdressStreet1", "Js-PostAdressStreet2", "Js-PostAdressStreet3", "Js-PostAdressZipCode", "Js-PostAdressCity"];
    var elementOutDataNames = ["Js-LivingAdressStreet1", "Js-LivingAdressStreet2", "Js-LivingAdressStreet3", "Js-LivingAdressZipCode", "Js-LivingAdressCity"];
    var elementParents = ["LAS1", "LAS2", "LAS3", "LAZC", "LAC"]
    var elementValues = [];
    var elementsForUser = ["Your Postadress line 1", "Your Postadress line 2", "Your Postadress line 3", "Your Postzipcode", "Your Posttown"];
    var checkedmessage = "";
    printErrorMessage("");
    for (var i = 0; i <= 4; i++) {
        elementValues.push(document.getElementById(elementInDataNames[i]).value)
    }
    var validData = validateData(elementValues);
    checkedmessage = errorMessage(validData, elementsForUser);
    if (checkedmessage !== "") {
        return;
    }

    for (i = 0; i <= 4; i++) {
        var output = document.createElement("input");
        output.setAttribute("type", "text");
        output.setAttribute("id", elementOutDataNames[i]);
        output.setAttribute("name", elementOutDataNames[i]);
        output.setAttribute("value", elementValues[i]);
        var itemToBeRemoved = document.getElementById(elementOutDataNames[i]);
        var parentItem = document.getElementById(elementParents[i]);
        parentItem.replaceChild(output, itemToBeRemoved);
    }
}

function isArray(x) {
    return x.constructor.toString().indexOf("Array") > -1;
}

function validateData(values) {
    var validData = [];
    if (isArray(values)) {
        for (var i = 0; i <= 4; i++) {
            if ((i !== 3) && (values[i].length >= 100)) {
                validData.push("Too many letters/numbers is given");
            } else if ((i !== 3) && (values[i].length === 0)) {
                validData.push("Too few letters/numbers is given");
            } else if ((i === 3) && (values[i].length < 5) || (values[i].length > 5)) {
                validData.push("Incorrect zipcode");
            } else {
                validData.push("Correct Data.");
            }
        }
    } else {
        validData.push("No values was read")
    }
    return validData;
}

function printErrorMessage(errorMessage) {
    var output = document.createElement("span");
    output.setAttribute("id", "validationMessage");
    output.innerText = errorMessage;
    var itemToBeRemoved = document.getElementById("validationMessage");
    var parentItem = document.getElementById("validationMessageParent");
    parentItem.replaceChild(output, itemToBeRemoved);
}

function errorMessage(validData, elementsForUser) {
    var errorProperties = "";
    for (var i = 0; i <= 4; i++) {
        if (validData[i] === "Too many letters/numbers is given") {
            errorProperties = errorProperties + validData[i] + " in " + elementsForUser[i] + ". \n";
        } else if (validData[i] === "Too few letters/numbers is given") {
            errorProperties = errorProperties + validData[i] + " in " + elementsForUser[i] + ". \n";
        } else if (validData[i] === "Incorrect zipcode") {
            errorProperties = errorProperties + validData[i] + " in " + elementsForUser[i] + ". \n";
        } else if (validData[i] === "No values was read") {
            errorProperties = errorProperties + validData[i] + " from the form, please try again. \n";
        }
    }

    if (errorProperties !== "") {
        printErrorMessage(errorProperties);
        return errorProperties;
    }
    return errorProperties;
}