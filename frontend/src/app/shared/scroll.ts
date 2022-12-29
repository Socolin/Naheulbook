export function smoothScrollBy(scrollX: number, scrollY: number, duration: number) {
    let currentOffset = window.pageYOffset;
    let targetOffset = scrollY;
    let start: number;
    window.requestAnimationFrame(function smoothScroll(timestamp: number) {
        if (!start) {
            start = timestamp;
        }

        let ellapsed = timestamp - start;
        let x = ellapsed / duration;
        if (x > 1) {
            x = 1;
        }
        else {
            window.requestAnimationFrame(smoothScroll);
        }

        let a = 4;
        let interpol = Math.pow(x, a) / (Math.pow(x, a) + Math.pow(1 - x, a));
        window.scrollTo(0, currentOffset + targetOffset * interpol);
    });
}

export function smoothScrollTo(scrollX: number, scrollY: number, duration: number) {
    let currentOffset = window.pageYOffset;

    smoothScrollBy(scrollX, scrollY - currentOffset, duration);
}
