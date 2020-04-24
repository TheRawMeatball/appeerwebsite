window.log = (obj) => {
   console.log(obj);
}

window.tooltip = () => {
    $('[data-toggle="tooltip"]').tooltip("dispose")
    $('[data-toggle="tooltip"]').tooltip()
}