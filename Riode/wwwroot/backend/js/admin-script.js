const deleteImageBtns = document.querySelectorAll('.deleteImageBtn');

deleteImageBtns.forEach(btn => {
    btn.addEventListener('click', function (e) {
        e.preventDefault();

        let url = this.getAttribute('href');

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Deleted!",
                            text: data.message,
                            icon: "success"
                        }).then(result => {
                            location.reload();
                        })
                    })
            }
        });
    });
});

const userBtn = document.querySelector('.UserBtn');
console.log(userBtn);
userBtn.addEventListener('click', function (e) {
e.preventDefault();

    let url = this.getAttribute('href');

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, do it!"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(url, { method: "POST" })
                .then(response => response.json())
                .then(data => {
                    Swal.fire({
                        title: "Success!",
                        text: data.message,
                        icon: "success"
                    }).then(result => {
                        location.reload();
                    })
                })
        }
    });
});

if (document.getElementById('addImage')) {
    document.getElementById('addImage').addEventListener('click', function () {
        var imageInputs = document.getElementById('imageInputs');
        if (imageInputs.children.length < 7) {
            var input = imageInputs.lastElementChild;
            if (!input || input.files.length > 0) {
                var clonedInput = input.cloneNode(true);
                clonedInput.value = '';
                imageInputs.appendChild(clonedInput);
            } else {
                alert('Select a file before adding another image');
            }
        } else {
            alert('You can only upload up to 5 images');
        }
    });
}


if (document.getElementById('addImageUpdate')) {
    document.getElementById('addImageUpdate').addEventListener('click', function () {
        var imageCount = document.querySelectorAll(".p-image").length;
        var productImageInputCount = document.querySelectorAll(".product-image-input").length;
        var imageInputs = document.getElementById("imageInputs");

        if (imageCount + productImageInputCount < 5) {
            var input = imageInputs.lastElementChild;
            if (!input || input.files.length > 0) {
                var clonedInput = input.cloneNode(true);
                clonedInput.value = '';
                imageInputs.appendChild(clonedInput);
            } else {
                alert('Select a file before adding another image');
            }
        } else {
            alert('You can only upload up to 5 images');
        }
    });
}


if (document.getElementById('addTopic')) {
    document.getElementById('addTopic').addEventListener('click', function () {
        var BlogTopics = document.getElementById('BlogTopics');
        if (BlogTopics.children.length < 6) {
            var input = BlogTopics.lastElementChild;
            if (!input || input.value !== '') {
                var clonedInput = input.cloneNode(true);
                clonedInput.value = '';
                BlogTopics.appendChild(clonedInput);
            } else {
                alert('Evvelce yuxaridakini doldur!');
            }
        } else {
            alert("You can't add more than four topics");
        }
    });
}
