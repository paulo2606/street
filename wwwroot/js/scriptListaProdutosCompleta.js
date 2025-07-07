const priceRange = document.querySelector('.price-range');
const priceValueSpan = document.getElementById('price-value');

if (priceRange && priceValueSpan) {
    priceValueSpan.textContent = priceRange.value;

    priceRange.addEventListener('input', () => {
        priceValueSpan.textContent = priceRange.value;
    });
}