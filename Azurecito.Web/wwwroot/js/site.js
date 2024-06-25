function openModal(src) {
    var modal = document.getElementById("myModal");
    var modalImg = document.getElementById("img01");
    var captionText = document.getElementById("caption");
    modal.style.display = "block";
    modalImg.src = src;
    captionText.innerHTML = "Foto";
}

function closeModal() {
    var modal = document.getElementById("myModal");
    modal.style.display = "none";
}