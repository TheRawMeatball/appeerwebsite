window.log = (obj) => {
   console.log(obj);
}

window.tooltip = () => {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}