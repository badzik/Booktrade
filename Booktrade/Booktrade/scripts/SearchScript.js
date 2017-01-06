$(document).ready(function ($) {
    $(".clickable-row").click(function () {
        window.document.location = $(this).data("href");
    });

    $("#ForSell").change(function () {
        if ($(this).is(":checked")) {
            $("#PriceFrom").prop('disabled', false);
            $("#PriceTo").prop('disabled', false);
        } else {
            $("#PriceFrom").val(null);
            $("#PriceTo").val(null);
            $("#PriceFrom").prop('disabled', true);
            $("#PriceTo").prop('disabled', true);
        }
    });

    $(".submitButton").click(function (event) {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });

    });

    $('#SortBy').on('change', function () {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    })

    $('#ResultsForPage').on('change', function () {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    })

    $(".myArrow").click(function (event) {
        var currentPage = parseInt($.trim($(".currentPage").text()), 10);
        var numberOfPages = parseInt($.trim($(".numberOfPages").text()), 10);
        var id = $(this).attr('id');
        if (id == "backArrow") {
            currentPage = currentPage - 1;
        }
        if (id == "backMaxArrow") {
            currentPage = 1;
        }
        if (id == "nextArrow") {
            currentPage = currentPage + 1;
        }
        if (id == "nextMaxArrow") {
            currentPage = numberOfPages;
        }
        $(".currentPage").text(currentPage);
        adjustPages();
        makeAjax();
    });

    var resetPages = function resetPages() {
        var numberOfPages = $("#MaxPages").val();
        $(".currentPage").text(1);
        $(".numberOfPages").text(numberOfPages);
    };

    var adjustPages = function adjustPages() {
        var currentPage = parseInt($.trim($(".currentPage").text()), 10);
        var numberOfPages = parseInt($.trim($(".numberOfPages").text()), 10);
        if (numberOfPages == 0) {
            $("#pageControl").hide();
        } else {
            $("#pageControl").show();
        }
        if (currentPage == 1) {
            $("#backArrow").hide();
            $("#backMaxArrow").hide();
        } else {
            $("#backArrow").show();
            $("#backMaxArrow").show();
        }
        if (currentPage == numberOfPages) {
            $("#nextMaxArrow").hide();
            $("#nextArrow").hide();
        } else {
            $("#nextMaxArrow").show();
            $("#nextArrow").show();
        }

    };

    var makeAjax = function makeAjax() {
        var model = {
            Phrase: $("#Phrase").val(),
            City: $("#City").val(),
            Province: $("#Province").val(),
            ForExchange: $('#ForExchange').is(':checked'),
            ForSell: $('#ForSell').is(':checked'),
            PriceFrom: $("#PriceFrom").val(),
            PriceTo: $("#PriceTo").val(),
            Publisher: $("#Publisher").val(),
            PublicationYear: $("#PublicationYear").val(),
            SortBy: $("#SortBy").val(),
            Category: $("#Category").val(),
            CurrentPage: $(".currentPage").text(),
            NumberOfPages: $(".numberOfPages").text(),
            ResultsForPage: $("#ResultsForPage").val()
        };

        return $.ajax({
            url: '/Search/SearchWindow',
            type: 'POST',
            data: model,
            datatype: "html",
            success: function (html) {
                $('.Results').replaceWith(html);
            },
            error: function () {
                alert("error");
            }
        });
    };

    var getUrlParameter = function getUrlParameter(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    }

    adjustPages();
    var par = getUrlParameter('category');
    $('a:contains(' + par + ')').css('font-weight', 'bold');
});