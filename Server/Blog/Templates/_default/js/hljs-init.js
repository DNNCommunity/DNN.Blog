(function () {
  function highlightCode() {
    if (!window.hljs) {
      return;
    }
    document.querySelectorAll('.bbs-article pre code, .blog-main-content pre code').forEach(function (block) {
      if (block.classList.contains('hljs')) {
        return;
      }
      hljs.highlightElement(block);
    });
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', highlightCode);
  } else {
    highlightCode();
  }
})();
