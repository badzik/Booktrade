$(document).ready( function() {
    	$(document).on('change', '.btn-file :file', function() {
		var input = $(this),
			label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
		input.trigger('fileselect', [label]);
		});

		//$('.btn-file :file').on('fileselect', function(event, label) {
		    
		//    var input = $(this).parents('.input-group0').find(':text'),
		//        log = label;
		    
		//    if( input.length ) {
		//        input.val(log);
		//    } else {
		//        if( log ) alert(log);
		//    }
	    
		//});
		function readURL(input,str) {
		    if (input.files && input.files[0]) {
		        var reader = new FileReader();
		        reader.onload = function (e) {
		            if (str == "add") {
		                $('#img-upload').attr('src', e.target.result);
		            }
		            if (str == "0") {
		                $('#img-upload0').attr('src', e.target.result);
		            }
		            if (str == "1") {
		                $('#img-upload1').attr('src', e.target.result);
		            }
		            if (str == "2") {
		                $('#img-upload2').attr('src', e.target.result);
		            }
		            if (str == "3") {
		                $('#img-upload3').attr('src', e.target.result);
		            }
		        }
		        
		        reader.readAsDataURL(input.files[0]);
		    }
		}
		if ($("#imgInp").change) {
		    $("#imgInp").change(function () {
                
		        readURL(this,"add");
		    });
		}
		if ($("#imgInp0").change) {
		    $("#imgInp0").change(function () {
		        readURL(this,"0");
		    });
		}
		if ($("#imgInp1").change) {
		    $("#imgInp1").change(function () {
		        readURL(this,"1");
		    });
		}
		if ($("#imgInp2").change) {
		    $("#imgInp2").change(function () {
		        readURL(this, "2");
		    });
		}
		if ($("#imgInp3").change) {
		    $("#imgInp3").change(function () {
		        readURL(this, "3");
		    });
		}

	});