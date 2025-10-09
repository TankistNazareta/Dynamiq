import React from 'react';
import './about.scss';

const About = () => {
    return (
        <section className="about-project">
            <div className="about-project__container">
                <h1 className="about-project__title">Dynamiq — Online Furniture Store</h1>

                <p className="about-project__description">
                    <strong>Dynamiq</strong> is an online furniture shop I made for my portfolio. I built everything
                    myself using <strong>.NET 8 (C#)</strong> for the backend and <strong>React with TypeScript</strong>{' '}
                    for the frontend. The project shows clean architecture, safe payments, and a modern user interface.
                </p>

                <h2 className="about-project__subtitle">Main Features</h2>
                <ul className="about-project__list">
                    <li className="about-project__item">
                        Product catalog with categories, filters, and search suggestions
                    </li>
                    <li className="about-project__item">Shopping cart with discounts and coupons</li>
                    <li className="about-project__item">Stripe payments: one-time and subscriptions (with webhooks)</li>
                    <li className="about-project__item">Secure login with JWT and Google</li>
                    <li className="about-project__item">Email confirmation and account management</li>
                    <li className="about-project__item">Simple admin panel</li>
                    <li className="about-project__item">Product recommendations</li>
                </ul>

                <h2 className="about-project__subtitle">Tech Stack</h2>
                <ul className="about-project__list">
                    <li className="about-project__item">
                        <strong>Backend:</strong> .NET 8, ASP.NET Core, EF Core, MediatR, FluentValidation, SQL Server
                    </li>
                    <li className="about-project__item">
                        <strong>Frontend:</strong> React, TypeScript, SCSS, Bootstrap
                    </li>
                    <li className="about-project__item">
                        <strong>Other:</strong> Stripe API, GitHub Actions
                    </li>
                </ul>

                <h2 className="about-project__subtitle">Architecture</h2>
                <p className="about-project__description">
                    The project uses <strong>Domain-Driven Design</strong>, <strong>Clean Architecture</strong>, and{' '}
                    <strong>CQRS</strong>. Business logic is separate from other parts. Integration tests run in
                    isolated SQL containers.
                </p>

                <h2 className="about-project__subtitle">My Role</h2>
                <p className="about-project__description">
                    I made the backend, frontend and database myself. This project shows my full-stack development
                    skills.
                </p>

                <h2 className="about-project__subtitle">Demo & Code</h2>
                <p className="about-project__description">
                    You can see the live site and full source code on{' '}
                    <a className="about-project__description-link" href="https://github.com/TankistNazareta/Dynamiq">
                        GitHub
                    </a>
                    .
                </p>
            </div>
        </section>
    );
};

export default About;
