import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  lang: 'zh-CN',
  titleTemplate: 'VNGod',
  title: "VNGod",
  description: "简洁有用的视觉小说管理器",
  sitemap: {
    hostname: 'https://vngod.samhou.moe'
  },
  lastUpdated: true,
  markdown: {
    lineNumbers: true
  },
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: '主页', link: '/' },
      { text: '文档', link: '/quick-start/description' },
      { text: '下载', link: '/download' }
    ],
    logo: 'favicon.ico',
    sidebar: [
      {
        text: '从这里开始',
        items: [
          { text: '简介', link: '/quick-start/description' },
          { text: '快速开始', link: '/quick-start/start' }
        ]
      },
      {
        text: '使用指南',
        items: [
          {text: '修改游戏信息',link:'/feature/game-edit'},
          {text: '图标搜刮',link:'/feature/icon'},
        ]
      },
      {
        text: '开发',
        items: [
          { text: '开发环境', link: '/community/development' },
          { text: '参与贡献', link: '/community/contribute' }
        ]
      }
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/SamHou0/VNGod' },
      { icon: 'bilibili', link: 'https://space.bilibili.com/456964528' },
      { icon: 'rss', link: 'https://blog.samhou.top' }
    ]
  }
})