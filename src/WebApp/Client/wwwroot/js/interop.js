window.interopFunctions = {
    textareaAutoHeight: function (textarea) {
        textarea.style.height = "5px";
        textarea.style.height = (textarea.scrollHeight) + "px";
    },
    copyElementHeight: function (fromId, toId) {
        let from = document.getElementById(fromId);
        let to = document.getElementById(toId);
        to.style.height = window.getComputedStyle(from).getPropertyValue('height');
    }
}