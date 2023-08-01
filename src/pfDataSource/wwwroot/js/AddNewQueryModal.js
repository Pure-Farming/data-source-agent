function addNewModal(show) {
    var modal = bootstrap.Modal.getOrCreateInstance('#addNewModal', { keyboard: false });
    if (show == "true") {
        console.log("pf :: showing modal");
        modal.show();
    } else {
        modal.hide();
        console.log("pf :: hiding modal");
    }
}
