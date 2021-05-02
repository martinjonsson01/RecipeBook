window.interopFunctions = {
    textareaAutoHeight: function (textarea) {
        textarea.style.height = "5px";
        textarea.style.height = (textarea.scrollHeight) + "px";
    },
    selectText: function(input) {
        input.select();
    }
}