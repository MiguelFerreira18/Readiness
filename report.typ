// ============================================================
// Software Requirements Specification (SRS) - Typst template
// Edit the variables below, then compile: typst compile srs.typ
// ============================================================

#let project = "Readiness"
#let version = "1.0"
#let author = "Miguel Ferreira, Miguel Souza, Bernardo Crista, Ana Silva"
#let date = datetime.today().display("[month repr:long] [day], [year]")
#let module = "C Sharp"
#let instructor = "Ricardo Canelas"

#set document(title: project + " - Software Requirements Specification", author: author)

#set page(
  paper: "a4",
  margin: (x: 2.5cm, y: 2.7cm),
  header: context {
    if counter(page).get().first() > 1 [
      #set text(size: 9pt, fill: gray)
      #project -- Software Requirements Specification #h(1fr) Version #version
      #v(-0.5em)
      #line(length: 100%, stroke: 0.5pt + gray)
    ]
  },
  footer: context {
    if counter(page).get().first() > 1 [
      #set text(size: 9pt, fill: gray)
      #line(length: 100%, stroke: 0.5pt + gray)
      #v(-0.5em)
      Confidential #h(1fr) Page #counter(page).display("1 of 1", both: true)
    ]
  },
)

#set text(font: "Libertinus Serif", size: 11pt)
#set par(justify: true, leading: 0.7em)
#set heading(numbering: "1.1.1")

#show heading.where(level: 1): it => {
  pagebreak(weak: true)
  v(0.5em)
  text(size: 18pt, weight: "bold", it)
  v(0.5em)
}
#show heading.where(level: 2): set text(size: 14pt)
#show heading.where(level: 3): set text(size: 12pt)
#show link: set text(fill: blue)
#show ref: set text(fill: blue)

// ---------- Helpers ----------

// A requirements table: pass rows as (id, description, priority)
#let req-table(..rows) = {
  let data = rows.pos()
  table(
    columns: (auto, 1fr),
    align: (left, left, center),
    stroke: 0.5pt + luma(180),
    fill: (_, y) => if y == 0 { luma(230) } else { none },
    inset: 7pt,
    [*ID*], [*Requirement*],
    ..data.flatten(),
  )
}

#let note(body) = block(
  fill: luma(245),
  stroke: (left: 3pt + gray),
  inset: 10pt,
  width: 100%,
  body,
)

// ---------- Title page ----------

#align(center + horizon)[
  #text(size: 28pt, weight: "bold")[Software Requirements \ Specification]
  #v(1em)
  #text(size: 18pt)[#project]
  #v(3em)
  #table(
    columns: (auto, auto),
    align: left,
    stroke: none,
    inset: 5pt,
    [*Version:*], [#version],
    [*Author:*], [#author],
    [*Date:*], [#date],
    [*Status:*], [Draft],
    [*Instructor:*], [#instructor]
  )
]

#pagebreak()

// ---------- Revision history ----------

#heading(outlined: false, numbering: none)[Revision History]

#table(
  columns: (auto, auto, auto, 1fr),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*Version*], [*Date*], [*Author*], [*Description of change*],
  [0.1], [2026-11-04], [all], [Initial draft],
  [1.0], [YYYY-MM-DD], [Your Name], [First release],
)

#v(2em)

// ---------- Table of contents ----------

#outline(title: [Table of Contents], depth: 3, indent: auto)

// ============================================================
// 1. Introduction
// ============================================================

= Introduction

== Purpose
The following document is presented as the final report for the work developed for #module DEV STREAM 2. Showing the ideas, work and processes used during the development of the project.

== Scope
The software product is called readiness (read + readiness) it is an identical software to Goodreads, and has its basic features to be considered a working MVP.
In this case, it allows users to sign in and sign up, it also allows them to add, remove books from their _read_ or _want to read_ list, it also allows users to add books to a whishlist
and more importantly it also allows users to review the books they have read, this in addition to the ability to add the time it took to read a certain book
will allow for different dashboards and data analysis.

For the moderation team, the idea is to allow them to add new books that aren't in the platform,
not only that it also allows them to moderate the reviews present in a certain book by removing certain bad reviews and finally, they can also approve or disapprove
of users requesting author status on the website.

== Definitions, Acronyms, and Abbreviations
#table(
  columns: (auto, 1fr),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*Term*], [*Definition*],
  [SRS], [Software Requirements Specification],
  [API], [Application Programming Interface],
  [UI], [User Interface],
  [REST], [Representational State Transfer],
)

== References
- IEEE Std 830-1998 / ISO/IEC/IEEE 29148:2018, Requirements engineering
- Other project documents (add here)

== Overview
Briefly explain how the rest of the document is organized.

// ============================================================
// 2. Overall Description
// ============================================================

= Overall Description

== Product Perspective
Explain how this product fits into the larger system or environment. Include a system context diagram if useful.

#figure(
  rect(width: 70%, height: 4cm, stroke: 0.5pt + gray)[
    #align(center + horizon)[_Insert system context diagram here_]
  ],
  caption: [System context diagram],
) <fig-context>

See @fig-context for the system context.

== Product Functions
Summarize the major functions the software will perform.

== User Classes and Characteristics
#table(
  columns: (auto, 1fr),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*User class*], [*Characteristics*],
  [Moderator], [Manages reviews, new books, Author submition tickets and system configuration.],
  [Author], [Has access to the core features of the API, but also indicate which books belong to them.],
  [Standard User], [Uses core features of the API.],
  [Logged off User], [Uses mainly the listings and views the reviews]
)

// == Operating Environment
// Hardware, operating systems, browsers, and other software the product must work with.

== Design and Implementation Constraints
The mandatory constraint is to have an API made with C sharp with dotnet framework,
outside of that we were given as much creativity freedom as we wanted. 

== Assumptions and Dependencies
Since its for show purpose some parts of the security are going to be ignored, for example csrf protection is one of the parts, not only that
but also allowing .env variables to be injected through a .env and not with the c sharp standard.

As for the dependencies, we are currently relying on a few, like Xunit for Unit testing, AspNet Authentication package
this allows us to make sign in and sign on's more secure, the DotNetEnv package which allows us to read automatically
the .env variables, making the development more smooth and finally the the OpenApi specification package, which
makes the API expose an endpoint showing a self-documenting user interface with all the paths the REST API provides.

// ============================================================
// 3. Specific Requirements
// ============================================================

= Specific Requirements

== Functional Requirements

=== User Authentication
#req-table(
  [RF-001], [O sistema deve permitir ao utilizador fazer sign up.],
  [RF-002], [O sistema deve permitir ao utilizador fazer sign in.],
  [RF-003], [O sistema deve permitir ao utilizador fazer logout.],
  [RF-004], [O sistema deve permitir ao utilizador recuperar a password.],
)

=== Data Management
#req-table(
  [RF-005], [O sistema deve permitir ao utilizador alterar as suas informações.],
  [RF-006], [O sistema deve permitir ao moderador bloquear um utilizador.],
  [RF-007], [O sistema deve permitir ao moderador adicionar um livro.],
  [RF-008], [O sistema deve permitir ao moderador remover uma review.],
  [RF-009], [O sistema deve permitir ao utilizador adicionar um livro há sua biblioteca.],
  [RF-010], [O sistema deve permitir ao utilizador remover um livro da sua biblioteca.],
  [RF-011], [O sistema deve permitir ao utilizador definir um livro como um desejo (wish).],
  [RF-012], [O sistema deve permitir ao utilizador remover um livro de ser um desejo (wish).],
  [RF-013], [O sistema deve permitir ao utilizador definir um livro como ‘lido’.],
  [RF-014], [O sistema deve permitir ao utilizador definir um livro como ‘para ler’.],
  [RF-015], [O sistema deve permitir ao utilizador adicionar uma review a um livro.],
  [RF-016], [O sistema deve obrigar ao utilizador dar um rating após efetuar uma review.],
  [RF-017], [O sistema deve permitir ao utilizador editar a sua review.],
  [RF-018], [O sistema deve permitir ao utilizador alterar o rating da sua review.],
  [RF-019], [O sistema deve permitir ao utilizador remover uma review sua.],
  [RF-020], [O sistema tem de mostrar quando uma review foi editada.],
  [RF-021], [O sistema tem de mostrar quando uma review foi apagada.],
  [RF-022], [O sistema deve permitir que a review seja vazia, mas tem de ter rating.],
  [RF-023], [O sistema deve permitir a utilizadores submeterem um ticket para serem autores.],
  [RF-024], [O sistema deve permitir ao moderador aceitar ou negar um ticket para um utilizador ser autor.],
  [RF-025], [O sistema deve permitir ao utilizador indicar, no ticket, os livros de que é autor.],
  [RF-026], [O sistema deve permitir ao moderador aceitar ou negar livros na lista do ticket (Ref: Req25).],
  [RF-027], [O sistema deve permitir ao utilizador definir uma meta de leitura.],
  [RF-028], [O sistema deve permitir ao utilizador ver se está ou não a cumprir com a meta de leitura definida.],
  [RF-029], [O sistema deve de enviar um e-mail a congratular o utilizador por ter atingido a sua meta de leitura.],
)

== External Interface Requirements

// === User Interfaces
// Describe screen layouts, navigation, accessibility, and UI standards.

// === Hardware Interfaces
// Describe supported devices and hardware interactions, if any.

=== Software Interfaces
Describe APIs, databases, third-party services, and protocols.
#table(
  columns: (auto, 1fr, auto),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*Interface*], [*Description*], [*Protocol*],
  [MySql 8.4], [Persistent storage for users, books, and reviews. The API reads and writes records on every CRUD request.], [SQL over TCP],
  [REST API (provided)], [Exposes endpoints for managing books, reviews, user data and for authentication. Consumed by any HTTP client, such as a web or mobile front end.], [HTTP, REST,JSON],
  [JWT], [Used to authenticate requests. The API issues a signed token on login and validates it on protected endpoints.], [HTTP `Authorization: Bearer` header],
)

=== Communication Interfaces
#table(
  columns: (auto, 1fr, auto),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*Aspect*], [*Description*], [*Standard*],
  [Transport], [All client communication happens over HTTP], [HTTP],
  [Message format], [Request and response bodis are JSON with the header `Content-Type: application/json`.], [JSON],
  [Authentication], [Clients send the JWT in the `Authorization` header on every protected endpoint.], [Bearer token (JWT)],
  [Error handling], [Standard HTTP status codes are used.], [HTTP status codes],
)

== Non-Functional Requirements

// === Performance
// #req-table(
//   [NFR-001], [The system shall respond to 95% of user requests within 2 seconds under normal load.], [High],
//   [NFR-002], [The system shall support at least 1,000 concurrent users.], [Medium],
// )

=== Security
#req-table(
  [NFR-001], [Request to add, modify or delete data shall always have a Bearer token.],
  [NFR-002], [Passwords shall be stored as an Hash of the original password],
)

// === Reliability and Availability
// #req-table(
//   [NFR-020], [The system shall have an uptime of at least 99.5% per month.], [Medium],
// )

// === Usability
// #req-table(
//   [NFR-030], [New users shall be able to complete core tasks without training.], [Medium],
// )

=== Maintainability
#req-table(
  [NFR-001], [The codebase shall have automated test coverage of at least 70%.],
)

// ============================================================
// 4. Data Requirements
// ============================================================

= Domain
This chapter explains the domain of the API being developed, it will discribe the entities and its relationship with the system.

== Entity relationship
@fig-erd shows the output of the conceptualization of the system's data and the relationships between that data. 

#figure(
  image("uml/db_erd_diagram.png", width: 80%),
  caption: [Database ERD diagram],
) <fig-erd>

The following chapters describe the data shown on the erd diagram.

== User

The user is the core of the API, he can have multiple roles that define what he can or can't do in the system. In essence, the normal User visualizes and interacts with the whole system,
this includes being able to add books onto a whishlist, being also able to add books as being _read_ or _to be read_, and also it can add a review of a given book.

If the user has other roles such as moderator role or author role he can have other functionalities on the API. As a moderator he can remove certain reviews, add books into the system and even approve
author role to be given. Finally, an user that is an author can do all the normal actions of the user but can associate his books to himself. 

== Role
The Role simply difines what authorizations another user might have on the system. In this case, it has 3 role types, the moderator role, the author role and the user role.
The user role is attributed to everyone making its sign up on the system as the default role.

== Book
The Book entity is also one of the core parts of the system. They contain information such as the name of the book, the summary, the publisher of the book, the year of release, the number of pages and the isbn.
This entity has a relationship with other major entities, such as the review entity and user. 

== Review

 

// ============================================================
// 5. Appendices
// ============================================================

= Appendices

== Appendix A: Use Cases
Add detailed use cases or user stories here.

== Appendix B: Open Issues
#table(
  columns: (auto, 1fr, auto),
  stroke: 0.5pt + luma(180),
  fill: (_, y) => if y == 0 { luma(230) } else { none },
  inset: 7pt,
  [*ID*], [*Issue*], [*Owner*],
  [TBD-1], [Describe an unresolved question.], [Name],
)
