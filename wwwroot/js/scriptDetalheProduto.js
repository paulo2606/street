document.addEventListener('DOMContentLoaded', () => {
    const detailHeaders = document.querySelectorAll('.detail-header');

    detailHeaders.forEach(header => {
        header.addEventListener('click', () => {
            const detailItem = header.closest('.detail-item');
            detailItem.classList.toggle('open');

            const icon = header.querySelector('i');
            if (detailItem.classList.contains('open')) {
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-up');
            } else {
                icon.classList.remove('fa-chevron-up');
                icon.classList.add('fa-chevron-down');
            }
        });
    });

    const colorBoxes = document.querySelectorAll('.color-box');
    colorBoxes.forEach(box => {
        box.addEventListener('click', () => {
            colorBoxes.forEach(b => b.classList.remove('active'));
            box.classList.add('active');
        });
    });

    const sizeBoxes = document.querySelectorAll('.size-box');
    sizeBoxes.forEach(box => {
        box.addEventListener('click', () => {
            sizeBoxes.forEach(b => b.classList.remove('active'));
            box.classList.add('active');
        });
    });
});