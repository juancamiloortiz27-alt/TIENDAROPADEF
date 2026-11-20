// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
    function maximumFor(element) {
        return element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement
            ? element.maxLength
            : -1;
    }

    function showLimit(element, limit) {
        const label = element.labels?.[0]?.textContent?.trim() || element.name || "Este campo";
        window.alert(`${label} permite un máximo de ${limit} caracteres.`);
    }

    document.addEventListener("beforeinput", (event) => {
        const element = event.target;
        const limit = maximumFor(element);
        if (limit < 0 || event.inputType.startsWith("delete") || event.data == null) return;

        const selectionLength = element.selectionEnd - element.selectionStart;
        if (element.value.length - selectionLength + event.data.length > limit) {
            event.preventDefault();
            showLimit(element, limit);
        }
    });

    document.addEventListener("paste", (event) => {
        const element = event.target;
        const limit = maximumFor(element);
        if (limit < 0) return;

        const pastedText = event.clipboardData?.getData("text") ?? "";
        const selectionLength = element.selectionEnd - element.selectionStart;
        if (element.value.length - selectionLength + pastedText.length > limit) {
            event.preventDefault();
            showLimit(element, limit);
        }
    });
})();