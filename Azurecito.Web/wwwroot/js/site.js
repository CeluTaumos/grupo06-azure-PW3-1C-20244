const dropArea = document.getElementById("dragArea"),
    dragText = dropArea.querySelector("h6"),
    browseButton = dropArea.querySelector("#browseButton"),
    fileInput = dropArea.querySelector("#fileInput"),
    confirmButton = document.getElementById("confirmButton");

let file;

browseButton.onclick = () => {
    fileInput.click();
}

fileInput.addEventListener("change", function () {
    file = this.files[0];
    dropArea.classList.add("active");
    viewFile();
});

dropArea.addEventListener("dragover", (event) => {
    event.preventDefault();
    dropArea.classList.add("active");
    dragText.textContent = "Soltar para subir el archivo";
});

dropArea.addEventListener("dragleave", () => {
    dropArea.classList.remove("active");
    dragText.textContent = "Arrastre y suelte la imagen";
});

dropArea.addEventListener("drop", (event) => {
    event.preventDefault();

    file = event.dataTransfer.files[0];
    viewFile();
});

function viewFile() {
    let fileType = file.type;
    let validExtensions = ["image/jpeg", "image/jpg", "image/png"];
    if (validExtensions.includes(fileType)) {
        let fileReader = new FileReader();
        fileReader.onload = () => {
            let fileURL = fileReader.result;
            let imgTag = `<img src="${fileURL}" alt="image">`;
            dropArea.innerHTML = imgTag;
            confirmButton.style.display = "block";
        }
        fileReader.readAsDataURL(file);
    } else {
        alert("Este no es un archivo de imagen ");
        dropArea.classList.remove("active");
        dragText.textContent = "Arrastre y suelte para cargar la imagen";
    }
}

confirmButton.addEventListener("click", () => { });
