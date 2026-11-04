// ============================================================
// Software Requirements Specification (SRS) - Typst template
// Edit the variables below, then compile: typst compile srs.typ
// ============================================================

#let project = "Readiness"
#let version = "1.0"
#let author = "Miguel Ferreira, Miguel Souza, Bernardo Crista, Ana Silva"
#let date = datetime.today().display("[month repr:long] [day], [year]")
#let module = "C Sharp"

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
    columns: (auto, 1fr, auto),
    align: (left, left, center),
    stroke: 0.5pt + luma(180),
    fill: (_, y) => if y == 0 { luma(230) } else { none },
    inset: 7pt,
    [*ID*], [*Requirement*], [*Priority*],
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
  [0.1], [2026-11-04], [*], [Initial draft],
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
The following document is presented as the final report for the work developed for #module DEV STREAM 2.
Showing the ideas, work and processes used during the development of the project. 

== Scope
Describe the software product: its name, what it will and will not do, and its main benefits and goals.

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
  [Administrator], [Manages users and system configuration.],
  [Standard user], [Uses core features of the application.],
)

== Operating Environment
Hardware, operating systems, browsers, and other software the product must work with.

== Design and Implementation Constraints
Regulatory requirements, technology mandates, hardware limits, etc.

== Assumptions and Dependencies
List factors assumed to be true and any external dependencies.

// ============================================================
// 3. Specific Requirements
// ============================================================

= Specific Requirements

== Functional Requirements

=== User Authentication
#req-table(
  [FR-001], [The system shall allow users to register with an email address and password.], [High],
  [FR-002], [The system shall allow registered users to log in and log out.], [High],
  [FR-003], [The system shall allow users to reset their password via email.], [Medium],
)

=== Data Management
#req-table(
  [FR-010], [The system shall allow users to create, view, edit, and delete records.], [High],
  [FR-011], [The system shall allow users to export data in CSV format.], [Low],
)

== External Interface Requirements

=== User Interfaces
Describe screen layouts, navigation, accessibility, and UI standards.

=== Hardware Interfaces
Describe supported devices and hardware interactions, if any.

=== Software Interfaces
Describe APIs, databases, third-party services, and protocols.

=== Communication Interfaces
Describe network protocols, message formats, and security.

== Non-Functional Requirements

=== Performance
#req-table(
  [NFR-001], [The system shall respond to 95% of user requests within 2 seconds under normal load.], [High],
  [NFR-002], [The system shall support at least 1,000 concurrent users.], [Medium],
)

=== Security
#req-table(
  [NFR-010], [All data in transit shall be encrypted using TLS 1.2 or higher.], [High],
  [NFR-011], [Passwords shall be stored using a salted, adaptive hash.], [High],
)

=== Reliability and Availability
#req-table(
  [NFR-020], [The system shall have an uptime of at least 99.5% per month.], [Medium],
)

=== Usability
#req-table(
  [NFR-030], [New users shall be able to complete core tasks without training.], [Medium],
)

=== Maintainability
#req-table(
  [NFR-040], [The codebase shall have automated test coverage of at least 80%.], [Low],
)

#note[
  *Tip:* Write each requirement so it is unambiguous, testable, and uniquely identified. Use "shall" for mandatory requirements.
]

// ============================================================
// 4. Appendices
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
