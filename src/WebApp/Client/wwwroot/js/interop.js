window.interopFunctions = {
    textareaAutoHeight: function (textarea) {
        textarea.style.height = "5px";
        textarea.style.height = (textarea.scrollHeight) + "px";
    },
    selectText: function(input) {
        input.select();
    },
    highlightDragLeave: function (e, element, id) {
        let rect = document.getElementById(id).getBoundingClientRect();
        if (e.clientY < rect.top || e.clientY >= rect.bottom || e.clientX < rect.left || e.clientX >= rect.right) {
            element.classList.remove("highlighted");
        }
    },
    dropEffectMovable: function(e) {
        e.stopPropagation();
        e.preventDefault();

        e.dataTransfer.dropEffect = 'move';
    }
}