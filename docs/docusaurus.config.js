// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

const PROJECT_NAME = "Wispo";

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: `Webinex / ${PROJECT_NAME}`,
  tagline: `Webinex / ${PROJECT_NAME}`,
  favicon: "img/favicon.ico",

  // Set the production url of your site here
  url: "https://webinex.github.io",
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: `/${PROJECT_NAME.toLowerCase()}/`,

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: "webinex", // Usually your GitHub org/user name.
  projectName: PROJECT_NAME.toLowerCase(), // Usually your repo name.
  deploymentBranch: "docs",
  trailingSlash: false,

  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",

  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },

  presets: [
    [
      "classic",
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl: `https://github.com/webinex/${PROJECT_NAME.toLowerCase()}/tree/main/docs/`,
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      // Replace with your project's social card
      image: "img/docusaurus-social-card.jpg",
      navbar: {
        title: "Webinex",
        logo: {
          alt: "Webinex Logo",
          src: "img/logo.svg",
          href: `/${PROJECT_NAME.toLowerCase()}/docs/getting-started`,
        },
        items: [
          {
            type: "docSidebar",
            sidebarId: "tutorialSidebar",
            position: "left",
            label: PROJECT_NAME,
          },
          {
            href: "https://webinex.github.io/starter-kit",
            label: "Webinex",
            position: "right",
          },
          {
            href: `https://github.com/webinex/${PROJECT_NAME.toLowerCase()}`,
            label: "GitHub",
            position: "right",
          },
        ],
      },
      footer: {
        style: "dark",
        copyright: `Copyright © ${new Date().getFullYear()} Webinex, Inc. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
        additionalLanguages: ["csharp"],
      },
    }),
};

module.exports = config;
