$(document).ready(function () {
    $("#searchInput").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "./Search/SearchByPhrase",
                type: "POST",
                dataType: "json",
                data: { phrase: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Title, value: item.Title };
                    }))

                },
                error: function () {
                    alert("error");
                }
            })
        },
        minLength: 2,
        messages: {
            noResults: "", results: function (resultsCount) { }
        }
    });
})