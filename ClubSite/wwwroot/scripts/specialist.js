function checkSpec(radio) {
    var url = new URL(window.location.href)
    const nameParam = radio.name; 
    const valueParam = radio.value; 
    if (valueParam == "all") {
        url.searchParams.delete(nameParam); 
    } else {
        url.searchParams.set(nameParam, valueParam);   
    } 
    window.location.href = url.toString()
}