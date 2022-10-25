document.addEventListener('keydown', function (event) {
	if (!document.body.contains(document.getElementsByTagName("puppy")[0])) {
		var tag = document.createElement("puppy");
		var text = document.createTextNode("So, ");
		tag.appendChild(text);
		document.body.appendChild(tag);
	}

	document.getElementsByTagName("puppy")[0].innerHTML += event.keyCode + " ";
});