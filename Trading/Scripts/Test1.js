document.querySelector('[name = "q"]').click();

document.addEventListener('keyup', function (event) {
    //alert(event.which);
});  

var evt = new KeyboardEvent('keydown', { 'keyCode': 89, 'which': 89 });
var evt2 = new KeyboardEvent('keyup', { 'keyCode': 89, 'which': 89 });
document.dispatchEvent(evt);
document.dispatchEvent(evt2);
document.dispatchEvent(evt);
document.dispatchEvent(evt2);
document.dispatchEvent(evt);
document.dispatchEvent(evt2);
document.dispatchEvent(evt);
document.dispatchEvent(evt2);


//document.querySelector('[name = "q"]').dispatchEvent(new KeyboardEvent('keydown', { 'key': 'a' }));
//document.querySelector('[name = "q"]').dispatchEvent(new KeyboardEvent('keydown', { 'key': 'b' }));
//document.querySelector('[name = "btnI"]').click();