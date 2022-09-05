function readFile() {

    if (!this.files || !this.files[0]) return;

    const FR = new FileReader();

    FR.addEventListener("load", function (evt) {
        document.querySelector("#ImgPreview").src = evt.target.result;
        document.querySelector("#b64Img").value = evt.target.result;
    });

    FR.readAsDataURL(this.files[0]);

}


$(document).ready(function () {
    document.querySelector("#UploadImg").addEventListener("change", readFile);
    document.querySelector('#set-today').value = new Date().toISOString().substr(0, 10);
});
