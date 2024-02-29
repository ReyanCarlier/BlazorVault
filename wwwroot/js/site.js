function isMobile() {
    return window.innerWidth <= 768;
}

window.blazorResize = {
    registerResizeCallback: function (dotnetReference) {
        window.addEventListener("resize", () => dotnetReference.invokeMethodAsync("OnBrowserResize"));
    },
    unregisterResizeCallback: function () {
        window.removeEventListener("resize", () => dotnetReference.invokeMethodAsync("OnBrowserResize"));
    }
};

// History.back
window.blazorHistory = {
    goBack: function () {
        window.history.back();
    }
};

window.copyTextToClipboard = function (text) {
    const textArea = document.createElement('textarea');
    textArea.style.position = 'fixed';
    textArea.style.opacity = '0';
    textArea.value = text;
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'réussi' : 'échoué';
        console.log('La copie du texte a ' + msg);
    } catch (err) {
        console.error('Impossible de copier le texte', err);
    }

    document.body.removeChild(textArea);
};

window.blazorReference = null;
function setBlazorReference(dotNetObj) {
    window.blazorReference = dotNetObj;
}

window.handleClickAndCopy = function (text) {
    navigator.clipboard.writeText(text).then(function () {
        if (window.blazorReference) {
            window.blazorReference.invokeMethodAsync("OnCopySuccess", text);
        }
    }).catch(function (error) {
        if (window.blazorReference) {
            window.blazorReference.invokeMethodAsync("OnCopyError", error);
        }
    });
}