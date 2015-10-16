function BlogService($, settings, mid) {
	var moduleId = mid;
	var baseServicepath = $.dnnSF(moduleId).getServiceRoot('Blog');

	this.ajaxCall = function (type, controller, action, id, data, success, fail) {
		// showLoading();
		$.ajax({
			type: type,
			url: baseServicepath + controller + '/' + action + (id != null ? '/' + id : ''),
			beforeSend: $.dnnSF(moduleId).setModuleHeaders,
			data: data
		}).done(function (retdata) {
			// hideLoading();
			if (success != undefined) {
				success(retdata);
			}
		}).fail(function (xhr, status) {
			// hideLoading();
			displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
			if (fail != undefined) {
				fail(xhr.responseText);
			}
		});
	}

	this.approvePost = function (blogId, postId, success) {
		this.ajaxCall('POST', 'Posts', 'Approve', null, { blogId: blogId, PostId: postId }, success);
	};

	this.deletePost = function (blogId, postId, success) {
		this.ajaxCall('POST', 'Posts', 'Delete', null, { blogId: blogId, PostId: postId }, success);
	};

	this.viewPost = function (blogId, postId, success) {
		this.ajaxCall('POST', 'Posts', 'View', null, { blogId: blogId, PostId: postId }, success);
	};

	this.approveComment = function (blogId, commentId, success) {
		this.ajaxCall('POST', 'Comments', 'Approve', null, { blogId: blogId, commentId: commentId, karma: 0 }, success);
	};

	this.deleteComment = function (blogId, commentId, success) {
		this.ajaxCall('POST', 'Comments', 'Delete', null, { blogId: blogId, commentId: commentId, karma: 0 }, success);
	};

	this.karmaComment = function (blogId, commentId, karma, success) {
		this.ajaxCall('POST', 'Comments', 'Karma', null, { blogId: blogId, commentId: commentId, karma: 0 }, function(data) {
			if (data.Result === 'exists') {
				// user already did this
			};
			if (success != undefined) {
				success();
			}
		});
	};

	this.addComment = function (blogId, postId, parentId, comment, author, website, email, success) {
		this.ajaxCall('POST', 'Comments', 'Add', null, { blogId: blogId, postId: postId, parentId: parentId, comment: comment, author: author, website: website, email: email }, success);
	};

	this.getCommentsHtml = function (blogId, postId, success) {
		this.ajaxCall('GET', 'Comments', 'List', null, { blogId: blogId, postId: postId }, success);
	};

	this.addModule = function (paneName, position, title, template, success) {
		this.ajaxCall('POST', 'Modules', 'Add', null, { paneName: paneName, position: position, title: title, template: template }, success);
	};

	this.exportBlog = function (blogId, success) {
		this.ajaxCall('POST', 'Blogs', 'Export', null, { blogId: blogId }, success);
	};

	this.getVocabularyML = function (vocabularyId, success) {
		this.ajaxCall('GET', 'Terms', 'VocabularyML', null, { vocabularyId: vocabularyId }, success);
	}

}

function displayMessage(msgBoxId, message, cssclass) {
	var messageNode = $("<div/>")
				   .addClass('dnnFormMessage ' + cssclass)
				   .text(message);
	$(msgBoxId).prepend(messageNode);
	messageNode.fadeOut(3000, 'easeInExpo', function () {
		messageNode.remove();
	});
};
