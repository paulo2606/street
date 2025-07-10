document.addEventListener('DOMContentLoaded', function () {
    const cadastroModal = document.getElementById('cadastroModal');
    const openCadastroModalBtn = document.getElementById('openCadastroModalBtn');
    const closeCadastroModalBtn = cadastroModal ? cadastroModal.querySelector('.modal-close-btn') : null;
    const switchToLoginModalLink = cadastroModal ? cadastroModal.querySelector('#switchToLoginModalLink') : null;

    const loginModal = document.getElementById('loginModal');
    const openLoginModalBtn = document.getElementById('openLoginModalBtn');
    const closeLoginModalBtn = loginModal ? loginModal.querySelector('.modal-close-btn') : null;
    const switchToCadastroModalLink = loginModal ? loginModal.querySelector('#switchToCadastroModalLink') : null;

    function openModal(modalElement) {
        if (modalElement) {
            modalElement.classList.add('active');
            document.body.classList.add('modal-open');
        }
    }

    function closeModal(modalElement) {
        if (modalElement) {
            modalElement.classList.remove('active');
            if (!document.querySelector('.modal-overlay.active')) {
                document.body.classList.remove('modal-open');
            }
        }
    }

    if (openCadastroModalBtn && cadastroModal) {
        openCadastroModalBtn.addEventListener('click', function (e) {
            e.preventDefault();
            closeModal(loginModal);
            openModal(cadastroModal);
        });
    }

    if (closeCadastroModalBtn && cadastroModal) {
        closeCadastroModalBtn.addEventListener('click', function () {
            closeModal(cadastroModal);
        });
    }

    if (cadastroModal) {
        cadastroModal.addEventListener('click', function (event) {
            if (event.target === cadastroModal) {
                closeModal(cadastroModal);
            }
        });
    }

    if (openLoginModalBtn && loginModal) {
        openLoginModalBtn.addEventListener('click', function (e) {
            e.preventDefault();
            closeModal(cadastroModal);
            openModal(loginModal);
        });
    }

    if (closeLoginModalBtn && loginModal) {
        closeLoginModalBtn.addEventListener('click', function () {
            closeModal(loginModal);
        });
    }

    if (loginModal) {
        loginModal.addEventListener('click', function (event) {
            if (event.target === loginModal) {
                closeModal(loginModal);
            }
        });
    }

    if (switchToCadastroModalLink && cadastroModal && loginModal) {
        switchToCadastroModalLink.addEventListener('click', function (e) {
            e.preventDefault();
            closeModal(loginModal);
            openModal(cadastroModal);
        });
    }

    if (switchToLoginModalLink && cadastroModal && loginModal) {
        switchToLoginModalLink.addEventListener('click', function (e) {
            e.preventDefault();
            closeModal(cadastroModal);
            openModal(loginModal);
        });
    }
});