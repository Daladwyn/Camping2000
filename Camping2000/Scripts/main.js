function JsCopyAdress() {
    console.log("function is reached");
    alert("Button is clicked");
    var LivingAdressStreet1 = document.getElementById('Js-PostAdressStreet1').value;
    var LivingAdressStreet2 = document.getElementById('Js-PostAdressStreet2').value;
    var LivingAdressStreet3 = document.getElementById('Js-PostAdressStreet3').value;
    var LivingAdressZipCode = document.getElementById('Js-PostAdressZipCode').value;
    var LivingAdressCity = document.getElementById('Js-PostAdressCity').value;
  
    

    //for (i = 1; i <= 3; i++) {
    //    var output = document.createElement("input");
    //    output.setAttribute("id", "Js-LivingAdressStreet" + i);
    //    output.setAttribute("type", "text");
    //    var string = "LivingAdressStreet" + i;
    //    console.log(string);
    //    output.setAttribute("value", string);
    //    var itemToBeRemoved = document.getElementById("Js-LivingAdressStreet" + i);
    //    //document.replaceChild(output, itemToBeRemoved);
    //}

    var output = document.createElement("input");
    output.setAttribute("type", "text");
    output.setAttribute("id", "Js-LivingAdressStreet1");
    output.setAttribute("name", "Js-LivingAdressStreet1");
    output.setAttribute("value", LivingAdressStreet1);
    console.log(output);
    var itemToBeRemoved = document.getElementById("Js-LivingAdressStreet1");
    document.div(LAS1).replaceChild(output, itemToBeRemoved);

    output = document.createElement("input");
    output.setAttribute("type", "text");
    output.setAttribute("id", "Js-LivingAdressStreet2");
    output.setAttribute("name", "Js-LivingAdressStreet2");
    output.setAttribute("value", LivingAdressStreet1);
    console.log(output);
    itemToBeRemoved = document.getElementById("Js-LivingAdressStreet2");
    document.div.replaceChild(output, itemToBeRemoved);

    output = document.createElement("input");
    output.setAttribute("type", "text");
    output.setAttribute("id", "Js-LivingAdressStreet3");
    output.setAttribute("name", "Js-LivingAdressStreet3");
    output.setAttribute("value", LivingAdressStreet1);
    console.log(output);
    itemToBeRemoved = document.getElementById("Js-LivingAdressStreet3");
    document.div.replaceChild(output, itemToBeRemoved);

    output = document.createElement("input");
    output.setAttribute("type", "text");
    output.setAttribute("id", "Js-LivingAdressZipCode");
    output.setAttribute("name", "Js-LivingAdressZipCode");
    output.setAttribute("value", LivingAdressStreet1);
    console.log(output);
    itemToBeRemoved = document.getElementById("Js-LivingAdressZipCode");
    document.div.replaceChild(output, itemToBeRemoved);

    output = document.createElement("input");
    output.setAttribute("type", "text");
    output.setAttribute("id", "Js-LivingAdressCity");
    output.setAttribute("name", "Js-LivingAdressCity");
    output.setAttribute("value", LivingAdressStreet1);
    console.log(output);
    itemToBeRemoved = document.getElementById("Js-LivingAdressCity");
    document.div.replaceChild(output, itemToBeRemoved);

    
}