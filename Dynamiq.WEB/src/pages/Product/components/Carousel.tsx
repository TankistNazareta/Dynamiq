import React, { useEffect, useState, useRef, ReactElement, ReactNode } from 'react';
import { JSX } from 'react/jsx-runtime';

interface CarouselProps {
    children: ReactNode;
    showSlides: number;
    imgs: JSX.Element[];
}

const Carousel: React.FC<CarouselProps> = ({ children, showSlides, imgs }) => {
    const sliderContent = useRef<HTMLDivElement | null>(null);
    const dotsRef = useRef<HTMLDivElement[]>([]);

    const [currentSlide, setCurrentSlide] = useState<number>(0);
    const [cantMove, setCantMove] = useState<boolean>(false);
    const [lastActiveDot, setLastActiveDot] = useState<number>(0);

    const childrenSlides = React.Children.map(children, (child) => {
        if (!React.isValidElement<React.HTMLAttributes<HTMLElement>>(child)) return null;

        return React.cloneElement(child, {
            className: child.props.className ? `${child.props.className} carousel-comp_slide` : 'carousel-comp_slide',
            style: { width: `${100 / showSlides}%` },
        });
    }) as React.ReactElement[];

    const dots = childrenSlides.map((_, i) => {
        if (i % showSlides === 0) {
            return (
                <div
                    className="carousel-comp__dot"
                    ref={(ref) => {
                        if (ref) dotsRef.current[i / showSlides] = ref;
                    }}
                    key={i}
                    onClick={() => setCurrentSlide(i)}>
                    {imgs[i]}
                </div>
            );
        }
        return null;
    });

    const fullChildrenList: ReactElement[] = [
        ...childrenSlides
            .slice(childrenSlides.length - showSlides, childrenSlides.length)
            .map((child, index) => React.cloneElement(child, { key: `clone-${index}` })),

        ...childrenSlides,

        ...childrenSlides.slice(0, showSlides).map((child, index) =>
            React.cloneElement(child, {
                key: `clone-${index + childrenSlides.length}`,
            })
        ),
    ];

    const ChangeSlideByBtn = (num: number) => {
        let newCurrent = currentSlide;
        newCurrent += num > 0 ? showSlides : -showSlides;
        setCurrentSlide(newCurrent);
    };

    useEffect(() => {
        if (cantMove || !sliderContent.current) return;

        sliderContent.current.style.left = (currentSlide / -showSlides) * 100 - 100 + '%';

        if (currentSlide < 0) {
            timeoutLastFirstSlide(childrenSlides.length - showSlides);
        } else if (currentSlide >= childrenSlides.length) {
            timeoutLastFirstSlide(0);
        } else {
            changeActiveDot(currentSlide);
            setLastActiveDot(currentSlide / showSlides);
        }
    }, [currentSlide]);

    const timeoutLastFirstSlide = (frstLast: number) => {
        if (!sliderContent.current) return;

        setCantMove(true);

        changeActiveDot(frstLast);
        setLastActiveDot(frstLast);

        setTimeout(() => {
            if (!sliderContent.current) return;

            sliderContent.current.style.transition = '0s all';
            sliderContent.current.style.left = (frstLast / -showSlides) * 100 - 100 + '%';

            setCurrentSlide(frstLast);

            setTimeout(() => {
                setCurrentSlide(frstLast);
                setCantMove(false);

                if (sliderContent.current) {
                    sliderContent.current.style.transition = '1s all';
                }
            }, 25);
        }, 975);
    };

    const changeActiveDot = (slide: number) => {
        if (dotsRef.current[lastActiveDot]) {
            dotsRef.current[lastActiveDot].classList.remove('carousel-comp__dot_active');
        }
        if (dotsRef.current[slide / showSlides]) {
            dotsRef.current[slide / showSlides].classList.add('carousel-comp__dot_active');
        }
    };

    let mouseStart: number | undefined;
    let wasTouch: boolean | undefined;

    const onStart = (
        e: React.TouchEvent<HTMLDivElement> | React.MouseEvent<HTMLDivElement>,
        trigger: 'touch' | 'mouse' = 'touch'
    ) => {
        if (cantMove) return;

        if (trigger === 'touch' && 'targetTouches' in e.nativeEvent) {
            mouseStart = e.nativeEvent.targetTouches[0].clientX;
        } else if (trigger === 'mouse' && 'clientX' in e) {
            wasTouch = true;
            mouseStart = e.clientX;
        }

        if (sliderContent.current) {
            sliderContent.current.style.transition = '0s all';
        }
    };

    const onMove = (
        e: React.TouchEvent<HTMLDivElement> | React.MouseEvent<HTMLDivElement>,
        trigger: 'touch' | 'mouse' = 'touch'
    ) => {
        let move: number | undefined;

        if (trigger === 'touch' && 'targetTouches' in e.nativeEvent) {
            move = e.nativeEvent.targetTouches[0].clientX;
        } else if (trigger === 'mouse' && 'clientX' in e && wasTouch) {
            move = e.clientX;
        }

        if (!move || cantMove || !sliderContent.current) return;
        if (mouseStart && (mouseStart - move > 400 || mouseStart - move < -400)) return;
        if (trigger !== 'touch' && !wasTouch) return;

        sliderContent.current.style.cursor = 'grabbing';
        sliderContent.current.style.userSelect = 'none';
        sliderContent.current.style.left =
            (currentSlide / -showSlides) * 100 - 100 - ((mouseStart ?? 0) - move) / 4 + '%';
    };

    const onEnd = (
        e: React.TouchEvent<HTMLDivElement> | React.MouseEvent<HTMLDivElement>,
        trigger: 'touch' | 'mouse' = 'touch'
    ) => {
        if (!sliderContent.current) return;

        sliderContent.current.style.transition = '1s all';

        let end: number = 0;

        if (trigger === 'touch' && 'changedTouches' in e.nativeEvent) {
            end = (mouseStart ?? 0) - e.nativeEvent.changedTouches[0].clientX;
        } else if (trigger === 'mouse' && 'clientX' in e) {
            wasTouch = false;
            end = (mouseStart ?? 0) - e.clientX;
        }

        if (end > 100) setCurrentSlide((current) => current + showSlides);
        else if (end < -100) setCurrentSlide((current) => current - showSlides);
        else {
            sliderContent.current.style.left = (currentSlide / -showSlides) * 100 - 100 + '%';
        }

        sliderContent.current.style.cursor = 'grab';
        sliderContent.current.style.userSelect = '';
    };

    return (
        <div className="d-flex carousel-comp">
            <div className="carousel-comp__dot_wrapper">{dots}</div>
            <div className="carousel-comp__inner">
                <button className="carousel-comp__btn carousel-comp__btn-prev" onClick={() => ChangeSlideByBtn(-1)}>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path
                            d="M16 4L8 12L16 20"
                            stroke="black"
                            strokeWidth="2"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                        />
                    </svg>
                </button>
                <div className="carousel-comp__wrapper_viewport">
                    <div
                        className="carousel-comp__track"
                        style={{ width: (fullChildrenList.length / showSlides) * 100 + '%' }}
                        ref={sliderContent}
                        onTouchStart={(e) => onStart(e, 'touch')}
                        onTouchMove={(e) => onMove(e, 'touch')}
                        onTouchEnd={(e) => onEnd(e, 'touch')}
                        onMouseUp={(e) => onEnd(e, 'mouse')}
                        onMouseDown={(e) => onStart(e, 'mouse')}
                        onMouseMove={(e) => onMove(e, 'mouse')}>
                        {fullChildrenList}
                    </div>
                </div>
                <button className="carousel-comp__btn carousel-comp__btn-next" onClick={() => ChangeSlideByBtn(1)}>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path
                            d="M8 4L16 12L8 20"
                            stroke="black"
                            strokeWidth="2"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                        />
                    </svg>
                </button>
            </div>
        </div>
    );
};

export default Carousel;
