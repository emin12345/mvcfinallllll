const loadMore = document.getElementById("showMoreBtn");
const productBox = document.getElementById("productBox");

let skip = 3;
loadMore.addEventListener("click", function () {
    let url = `/Shop/LoadMore?skip=${skip}`;
    console.log(url);
    fetch(url).then(response => response.text())
        .then(data => productBox.innerHTML += data);

    skip += 3;

});