
export function showToast(eleName, show) {
    const toastElement = document.getElementById(eleName)

    if (show) {
        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastElement)
        toastBootstrap.show();
    }
}